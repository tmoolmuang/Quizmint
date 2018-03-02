using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quizmint;

namespace Quizmint.Controllers
{
    public class AssignmentAnswersController : Controller
    {
        private ShamuEntities db = new ShamuEntities();

        // GET: AssignmentAnswers
        public ActionResult Index()
        {
            var assignmentAnswers = db.AssignmentAnswers.Include(a => a.Assignment).Include(a => a.Question);
            return View(assignmentAnswers.ToList());
        }

        // GET: AssignmentAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignmentAnswer assignmentAnswer = db.AssignmentAnswers.Find(id);
            if (assignmentAnswer == null)
            {
                return HttpNotFound();
            }
            return View(assignmentAnswer);
        }

        // GET: AssignmentAnswers/Create
        public ActionResult Create()
        {
            ViewBag.AssignmentId = new SelectList(db.Assignments, "Id", "Id");
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QuestionText");
            return View();
        }

        // POST: AssignmentAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AssignmentId,QuestionId,UserSelectedAnswerId,UserSelectedTrue,UserEnteredText,IsCorrect")] AssignmentAnswer assignmentAnswer)
        {
            if (ModelState.IsValid)
            {
                db.AssignmentAnswers.Add(assignmentAnswer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignmentId = new SelectList(db.Assignments, "Id", "Id", assignmentAnswer.AssignmentId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QuestionText", assignmentAnswer.QuestionId);
            return View(assignmentAnswer);
        }

        // GET: AssignmentAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignmentAnswer assignmentAnswer = db.AssignmentAnswers.Find(id);
            if (assignmentAnswer == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignmentId = new SelectList(db.Assignments, "Id", "Id", assignmentAnswer.AssignmentId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QuestionText", assignmentAnswer.QuestionId);
            return View(assignmentAnswer);
        }

        // POST: AssignmentAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AssignmentId,QuestionId,UserSelectedAnswerId,UserSelectedTrue,UserEnteredText,IsCorrect")] AssignmentAnswer assignmentAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignmentAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignmentId = new SelectList(db.Assignments, "Id", "Id", assignmentAnswer.AssignmentId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QuestionText", assignmentAnswer.QuestionId);
            return View(assignmentAnswer);
        }

        // GET: AssignmentAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssignmentAnswer assignmentAnswer = db.AssignmentAnswers.Find(id);
            if (assignmentAnswer == null)
            {
                return HttpNotFound();
            }
            return View(assignmentAnswer);
        }

        // POST: AssignmentAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AssignmentAnswer assignmentAnswer = db.AssignmentAnswers.Find(id);
            db.AssignmentAnswers.Remove(assignmentAnswer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
