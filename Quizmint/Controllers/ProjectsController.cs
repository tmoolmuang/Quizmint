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

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            // 1. project id parameter must be provided
            // 2. project record must exist in db
            // 3. maker id session must exist, and maker must be owner of project
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (Session["MakerId"] == null || !Helper.IsProjectOwner(Int32.Parse(Session["MakerId"].ToString()), (int)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Session["ProjectId"] = id;
            Session["ProjectName"] = project.ProjectName;
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            Session["ProjectId"] = null;
            Session["ProjectName"] = null;

            if (Session["MakerId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Must login");
            }
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectName")] Project project)
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

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (Session["MakerId"] == null || !Helper.IsProjectOwner(Int32.Parse(Session["MakerId"].ToString()), (int)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectName")] Project project)
        {
            project.MakerId = Int32.Parse(Session["MakerId"].ToString());
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (Session["MakerId"] == null || !Helper.IsProjectOwner(Int32.Parse(Session["MakerId"].ToString()), (int)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
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
