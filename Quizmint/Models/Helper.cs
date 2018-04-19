using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quizmint.Models
{
    public class Helper
    {
        private static ShamuEntities db = new ShamuEntities();

        public static bool IsProjectOwner(int makerId, int projectId)
        {
            string myQuery =    "Select Maker.* From Maker, Project " +
                                "Where Maker.Id = Project.MakerId " +
                                "And Project.Id = " + projectId.ToString() + " " +
                                "And Maker.Id = " + makerId.ToString();

            return db.Database.SqlQuery<Maker>(myQuery).Count() > 0 ? true : false;
        }

        public static bool IsQuestionOwner(int makerId, int questionId)
        {
            string myQuery =    "Select Maker.* From Maker, Project, Question " +
                                "Where Project.Id = Question.ProjectId " +
                                "And Maker.Id = Project.MakerId " +
                                "And Question.Id = " + questionId.ToString() + " " +
                                "And Maker.Id = " + makerId.ToString();

            return db.Database.SqlQuery<Maker>(myQuery).Count() > 0 ? true : false;
        }

        public static bool IsAnswerOwner(int makerId, int answerId)
        {
            string myQuery =    "Select Maker.* From Maker, Project, Question, Answer " +
                                "Where Question.Id = Answer.QuestionId " +
                                "And Project.Id = Question.ProjectId " +
                                "And Maker.Id = Project.MakerId " +
                                "And Answer.Id = " + answerId.ToString() + " " +
                                "And Maker.Id = " + makerId.ToString();

            return db.Database.SqlQuery<Maker>(myQuery).Count() > 0 ? true : false; 
        }

        public static void SetAllChoiceToFalse(int questionId, int answerId)
        {
            string myQuery =    "Update Answer " +
                                "Set IsCorrectAnswer = 0 " +
                                "Where Answer.QuestionId = " + questionId.ToString() + " " +
                                "And Answer.Id != " + answerId.ToString();

            db.Database.ExecuteSqlCommand(myQuery);
        }
    }
}