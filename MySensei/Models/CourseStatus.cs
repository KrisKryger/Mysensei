using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySensei.Models
{
    public class CourseStatus
    {
        public int CourseStatusId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public ICollection<Course> Courses { get; set; }
        
    }
}