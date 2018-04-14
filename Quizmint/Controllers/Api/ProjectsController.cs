using Quizmint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;

namespace Quizmint.Controllers.Api
{
    public class ProjectsController : ApiController
    {
        private ShamuEntities db = new ShamuEntities();

        //GET: api/projects
        public IEnumerable<Project> Get()
        {
            return db.Projects.ToList();
        }

        //GET: api/projects/1
        public IHttpActionResult Get(int id)
        {
            Project project = db.Projects.Find(id);

            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        //POST: api/projects
        [System.Web.Http.HttpPost]
        public IHttpActionResult Create(Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                project.CreatedDate = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return Ok(project);
            }
        }

        //PUT: api/projects/1
        [System.Web.Http.HttpPut]
        public IHttpActionResult Update(int id, Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                Project orgProject = db.Projects.Find(id);
                if (orgProject == null)
                {
                    return NotFound();
                }
                else
                {
                    orgProject.ProjectName = project.ProjectName;
                    db.SaveChanges();
                    return Ok(orgProject);
                }
            }
        }

        //DELETE: api/projects/1
        [System.Web.Http.HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            else
            {
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
                return Ok();
            }
        }
    }
}