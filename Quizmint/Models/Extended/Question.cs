using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quizmint.Models
{
    [MetadataType(typeof(QuestionMetaData))]
    public partial class Question
    { }

    public class QuestionMetaData
    {
        [Display(Name = "Question Type")]
        public int QuestionTypeId { get; set; }

        [Display(Name = "Question Text")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter question")]
        public string QuestionText { get; set; }

        [Display(Name = "Answer")]
        public bool IsTrue { get; set; }
    }
}