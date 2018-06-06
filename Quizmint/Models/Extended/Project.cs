using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quizmint.Models
{
    [MetadataType(typeof(ProjectMetaData))]
    public partial class Project
    { }

    public class ProjectMetaData
    {
        [Display(Name = "Project Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter project name")]
        public string ProjectName { get; set; }

        [Display(Name = "Project Description")]
        public string ProjectDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }
    }
}