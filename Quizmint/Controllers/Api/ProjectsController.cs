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
        public IEnumerable<Project> GetProject()
        {
            return db.Projects.ToList();
        }

        //GET: api/projects/1
        public Project GetProject(int id)
        {
            Project project = db.Projects.SingleOrDefault(p => p.Id == id);

            if (project == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return project;
        }

        //POST: api/projects
        [System.Web.Http.HttpPost]
        public Project CreateProject(Project project)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            else
            {
                project.CreatedDate = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return project;
            }
        }

        //PUT: api/projects/1
        [System.Web.Http.HttpPut]
        public void UpdateProject(int id, Project project)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            else
            {
                Project orgProject = db.Projects.SingleOrDefault(p => p.Id == id);
                if (orgProject == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                else
                {
                    orgProject.ProjectName = project.ProjectName;
                    db.SaveChanges();
                }
            }
        }

        //DELETE: api/projects/1
        [System.Web.Http.HttpDelete]
        public void DeleteProject(int id)
        {
            Project project = db.Projects.SingleOrDefault(p => p.Id == id);
            if (project == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
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
            }
        }
    }
}