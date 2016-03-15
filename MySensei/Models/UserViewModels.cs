using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MySensei.Models
{
    // Users
    public class CreateModel
    {
        public string PhoneNumber { get; set; }

        [Display(Name = "Vej")]
        [StringLength(128)]
        public string Street { get; set; }

        [Display(Name = "Postnummer")]
        [StringLength(128)]
        public string Zipcode { get; set; }

        [Display(Name = "By")]
        [StringLength(128)]
        public string City { get; set; }

        [Display(Name = "Fødselsdag")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoinDate { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Fulde navn")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Kodeord")]
        [StringLength(256)]
        public string Password { get; set; }

        [Compare("Password")]
        [Display(Name = "Gentag Kodeord")]
        [StringLength(256)]
        public string RepeatPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Kodeord")]
        [StringLength(256)]
        public string Password { get; set; }
    }
    // Roles
    public class RoleEditModel
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}