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

        public ActionResult Index(int? id)
        {
            Session["ProjectId"] = null;
            Session["ProjectName"] = null;

            //projects by maker
            if (id == null || Int32.Parse(Session["MakerId"].ToString()) != id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var projects = db.Projects.Include(p => p.Maker).Where(p => p.MakerId == id);
            return View(projects.ToList());
        }


        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);
            if (project == null || project.MakerId != Int32.Parse(Session["MakerId"].ToString()))
            {
                return HttpNotFound();
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
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectName,MakerId,QuestionCount")] Project project)
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
            if (project == null || project.MakerId != Int32.Parse(Session["MakerId"].ToString()))
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectName,MakerId,QuestionCount")] Project project)
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
            if (project == null || project.MakerId != Int32.Parse(Session["MakerId"].ToString()))
            {
                return HttpNotFound();
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
            return RedirectToAction("Index", new { id = Int32.Parse(Session["MakerId"].ToString()) });
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
