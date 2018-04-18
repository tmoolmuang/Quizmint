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

            // 1. project id parameter must be provided
            // 2. maker id session must exist, and maker must be owner of project

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Session["MakerId"] == null || !Helper.IsProjectOwner(Int32.Parse(Session["MakerId"].ToString()), (int)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // when call from project index, project id has not been set into session
            if (Session["ProjectId"] == null)
            {
                Session["ProjectId"] = id;
                Session["ProjectName"] = db.Projects.Find(id).ProjectName;
            }

            // questions by project
            List<Question> questions = db.Questions.Include(q => q.Project).Where(q => q.ProjectId == id).Include(q => q.QuestionType).ToList();
            return View(questions);
        }

        // GET: Questions/Create
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

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,QuestionText,QuestionTypeId,IsTrue,NumberOfChoice")] Question question)
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

        // GET: Questions/Edit/5
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
                !Helper.IsQuestionOwner(Int32.Parse(Session["MakerId"].ToString()), (int)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            ViewBag.QuestionTypeId = new SelectList(db.QuestionTypes, "Id", "Name", question.QuestionTypeId);

            Session["QuestionId"] = id;
            Session["QuestionText"] = question.QuestionText;
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,QuestionText,QuestionTypeId,IsTrue,NumberOfChoice")] Question question)
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

        // GET: Questions/Delete/5
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
                !Helper.IsQuestionOwner(Int32.Parse(Session["MakerId"].ToString()), (int)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
