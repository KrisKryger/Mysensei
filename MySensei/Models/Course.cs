using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySensei.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string AppUserId { get; set; } //Users: Id

        [Required]
        [Display(Name = "Titel")]
        [StringLength(128)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(2048, MinimumLength = 125)]
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Erfaring")]
        public int Experience { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }

        public int CourseStatusId { get; set; } //CourseStatus: Id

        public virtual AppUser AppUser { get; set; }
        public virtual CourseStatus CourseStatus { get; set; }
        
        public virtual ICollection<AppUser> AppUsers { get; set; }
        public Course()
        {
            this.AppUsers = new HashSet<AppUser>();
        }

    }
}