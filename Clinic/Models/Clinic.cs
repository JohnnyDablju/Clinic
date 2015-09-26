using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Medcare.Models
{
    public class Clinic
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}