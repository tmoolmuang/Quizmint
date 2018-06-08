using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quizmint.Models;

namespace Quizmint.Controllers
{
    [Authorize]
    public class AnswersController : Controller
    {
        private ShamuEntities db = new ShamuEntities();

        public ActionResult Index(int? id)
        {
            if (id == null || 
                Session["ProjectId"] == null ||
                Session["QuestionId"] == null ||
                Int32.Parse(Session["QuestionId"].ToString()) != id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //answers by question
            List<Answer> answers = db.Answers.Include(a => a.Question).Where(a => a.QuestionId == id).ToList();
            return View(answers);
        }

        public ActionResult _index(int id)
        {
            List<Answer> answers = db.Answers.Include(a => a.Question).Where(a => a.QuestionId == id).ToList();
            return PartialView(answers);
        }

        public ActionResult _delete(int id)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
            db.SaveChanges();

            return RedirectToAction("index", new { id = answer.QuestionId });
        }

        public ActionResult Create()
        {
            if (Session["MakerId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Must login");
            }

            if (Session["ProjectId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Must define project id");
            }

            if (Session["QuestionId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Must define question id");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnswerText,IsCorrectAnswer")] Answer answer)
        {
            answer.QuestionId = Int32.Parse(Session["QuestionId"].ToString());
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                if (answer.IsCorrectAnswer)
                {
                    Helper.SetAllChoiceToFalse(answer.QuestionId, answer.Id);
                }
                return RedirectToAction("Index", new { id = answer.QuestionId });
            }

            return View(answer);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }

            if (Session["MakerId"] == null ||
                Session["ProjectId"] == null ||
                Session["QuestionId"] == null ||
                Int32.Parse(Session["MakerId"].ToString()) != answer.Question.Project.MakerId ||
                Int32.Parse(Session["ProjectId"].ToString()) != answer.Question.Project.Id ||
                Int32.Parse(Session["QuestionId"].ToString()) != answer.Question.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(answer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();

                if (answer.IsCorrectAnswer)
                {
                    Helper.SetAllChoiceToFalse(answer.QuestionId, answer.Id);
                }
                return RedirectToAction("Index", new { id = answer.QuestionId });
            }
            return View(answer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}