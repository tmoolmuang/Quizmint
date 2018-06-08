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
    public class QuestionsController : Controller
    {
        private ShamuEntities db = new ShamuEntities();

        public ActionResult Index(int? id)
        {
            Session["QuestionId"] = null;
            Session["QuestionText"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Session["MakerId"] == null || 
                db.Projects.Find(id).MakerId != Int32.Parse(Session["MakerId"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // when call question list from project index, project id has not been set into session yet,
            // needed for Title bar
            if (Session["ProjectId"] == null)
            {
                Session["ProjectId"] = id;
                Session["ProjectName"] = db.Projects.Find(id).ProjectName;
            }

            // questions by project
            List<Question> questions = db.Questions.Include(q => q.Project).Where(q => q.ProjectId == id).Include(q => q.QuestionType).ToList();
            return View(questions);
        }

        public ActionResult Create()
        {
            Session["QuestionId"] = null;
            Session["QuestionText"] = null;

            if (Session["MakerId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Must login");
            }

            if (Session["ProjectId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Must define project id");
            }

            ViewBag.QuestionTypeId = new SelectList(db.QuestionTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuestionText,QuestionTypeId,IsTrue,NumberOfChoice")] Question question)
        {
            question.ProjectId = Int32.Parse(Session["ProjectId"].ToString());
            question.NumberOfChoice = question.QuestionTypeId != 1 ? null : question.NumberOfChoice;
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = question.ProjectId });
            }

            ViewBag.QuestionTypeId = new SelectList(db.QuestionTypes, "Id", "Name", question.QuestionTypeId);
            return View(question);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (Session["MakerId"] == null ||
                Session["ProjectId"] == null ||
                Int32.Parse(Session["ProjectId"].ToString()) != question.Project.Id ||
                Int32.Parse(Session["MakerId"].ToString()) != question.Project.MakerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.QuestionTypeId = new SelectList(db.QuestionTypes, "Id", "Name", question.QuestionTypeId);

            Session["QuestionId"] = id;
            Session["QuestionText"] = question.QuestionText;
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Question question)
        {
            question.NumberOfChoice = question.QuestionTypeId != 1 ? null : question.NumberOfChoice;
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = question.ProjectId });
            }

            ViewBag.QuestionTypeId = new SelectList(db.QuestionTypes, "Id", "Name", question.QuestionTypeId);
            return View(question);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (Session["MakerId"] == null ||
                Session["ProjectId"] == null ||
                Int32.Parse(Session["ProjectId"].ToString()) != question.Project.Id ||
                Int32.Parse(Session["MakerId"].ToString()) != question.Project.MakerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // cascade delete related answers
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Answer[] answers = db.Answers.Where(a => a.QuestionId == id).ToArray();
            foreach (Answer a in answers)
            {
                db.Answers.Remove(a);
            }

            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = question.ProjectId });
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