using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySensei.Models
{
    public class AppUser: IdentityUser
    {
        [StringLength(128)]
        public string Street { get; set; }

        [StringLength(128)]
        public string Zipcode { get; set; }

        [StringLength(128)]
        public string City { get; set; }

        [StringLength(2048)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoinDate { get; set; }
        
        public ICollection<Course> CreatedCourses { get; set; }
        public ICollection<Course> Courses { get; set; }
        public AppUser()
        {
            Courses = new HashSet<Course>();
        }
        

    }
}