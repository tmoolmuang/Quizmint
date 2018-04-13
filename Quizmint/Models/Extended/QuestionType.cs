using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quizmint.Models
{
    [MetadataType(typeof(QuestionTypeMetaData))]
    public partial class QuestionType
    {}

    public class QuestionTypeMetaData
    {
        [Display(Name = "Question Type")]
        public string Name { get; set; }
    }
}