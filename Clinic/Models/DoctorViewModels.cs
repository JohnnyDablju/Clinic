using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Medcare.Models
{
    public class CreateWorkdayViewModel
    {
        [Display(Name = "Day of week")]
        [Required]
        public DayOfWeek Day { get; set; }

        [Display(Name = "Start hour")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[1-3]):[0|3][0]$", ErrorMessage = "Invalid hour format. Please use hh:00 or hh:30.")]
        [Required]
        public string StartHour { get; set; }

        [Display(Name = "End hour")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[1-3]):[0|3][0]$", ErrorMessage = "Invalid hour format. Please use hh:00 or hh:30.")]
        [Required]
        public string EndHour { get; set; }

        [Required]
        public IEnumerable<SelectListItem> ClinicsList { get; set; }
    }
}