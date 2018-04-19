using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quizmint.Models
{
    [MetadataType(typeof(AnswerMetaData))]
    public partial class Answer
    {}

    public class AnswerMetaData
    {
        [Display(Name = "Answer Choice")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter answer choice")]
        public string AnswerText { get; set; }

        [Display(Name = "Correct Answer")]
        public bool IsCorrectAnswer { get; set; }
    }


}