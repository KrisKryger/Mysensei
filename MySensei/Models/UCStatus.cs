using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MySensei.Models
{
    public class UCStatus
    {
        public int UCStatusId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}