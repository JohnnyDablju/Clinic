using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using IdentitySample.Models;

namespace Medcare.Models
{
    public class ScheduleVisitViewModel
    {
        [Required]
        public IEnumerable<SelectListItem> ClinicsList { get; set; }
        [Required]
        public IEnumerable<SelectListItem> DoctorsList { get; set; }
    }

    public class ScheduleVisitAdvancedViewModel
    {
        [Required]
        public IEnumerable<SelectListItem> DoctorsList { get; set; }

        [Display(Name="Date")]
        [RegularExpression(@"^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$", ErrorMessage="Invalid date format. Please use dd/mm/yyyy")]
        [Required]
        public string Date { get; set; }

        [Display(Name="Time")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[1-3]):[0|3][0]$", ErrorMessage = "Invalid hour format. Please use hh:00 or hh:30.")]
        [Required]
        public string Hour { get; set; }
    }

    public class VisitPropositionViewModel
    {
        public Guid ClinicId { get; set; }
        public string DoctorId { get; set; }
        public DateTime StartDateTime { get; set; }

        public Clinic Clinic { get; set; }
        public ApplicationUser Doctor { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }
        [Display(Name="Day of week")]
        public DayOfWeek Day { get; set; }
    }

    public class VisitProposition
    {
        public DateTime StartDateTime { get; set; }
        public Guid ClinicId { get; set; }
        public string DoctorId { get; set; }
    }
}