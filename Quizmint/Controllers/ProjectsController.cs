using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quizmint.Models;

namespace Quizmint.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ShamuEntities db = new ShamuEntities();

        public ActionResult Index()
        {
            Session["ProjectId"] = null;
            Session["ProjectName"] = null;

            if (Session["MakerId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Must login");
            }

            int makerId = Int32.Parse(Session["MakerId"].ToString());
            var projects = db.Projects.Include(p => p.Maker).Where(p => p.MakerId == makerId);
            return View(projects.ToList());
        }

        public ActionResult Details(int? id)
        {
            Session["QuestionId"] = null;
            Session["QuestionText"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);
            if (project == null || project.MakerId != Int32.Parse(Session["MakerId"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Session["ProjectId"] = id;
            Session["ProjectName"] = project.ProjectName;
            return View(project);
        }

        public ActionResult Create()
        {
            Session["ProjectId"] = null;
            Session["ProjectName"] = null;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectName,ProjectDescription")] Project project)
        {
            project.MakerId = Int32.Parse(Session["MakerId"].ToString());
            if (ModelState.IsValid)
            {
                project.CreatedDate = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            return View(project);
        }

        public ActionResult Edit(int? id)
        {
            Session["QuestionId"] = null;
            Session["QuestionText"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);
            if (project == null || project.MakerId != Int32.Parse(Session["MakerId"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            return View(project);
        }

        public ActionResult Delete(int? id)
        {
            Session["QuestionId"] = null;
            Session["QuestionText"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);
            if (project == null || project.MakerId != Int32.Parse(Session["MakerId"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Session["ProjectId"] = id;
            Session["ProjectName"] = project.ProjectName;
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // cascade delete related questions and answers
            Project project = db.Projects.Find(id);
            Question[] questions = db.Questions.Where(q => q.ProjectId == id).ToArray();
            foreach (Question q in questions)
            {
                Answer[] answers = db.Answers.Where(a => a.QuestionId == q.Id).ToArray();
                foreach (Answer a in answers)
                {
                    db.Answers.Remove(a);
                }
                db.Questions.Remove(q);
            }
            db.Projects.Remove(project);
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