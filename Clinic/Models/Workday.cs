using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentitySample.Models;
using System.ComponentModel.DataAnnotations;

namespace Medcare.Models
{
    public class Workday
    {
        public Guid Id { get; set; }
        [Display(Name = "Day of week")]
        public DayOfWeek Day { get; set; }
        [Display(Name = "Start hour")]
        [DataType(DataType.Time)]
        public DateTime StartHour { get; set; }
        [Display(Name = "End hour")]
        [DataType(DataType.Time)]
        public DateTime EndHour { get; set; }
        
        public string DoctorId { get; set; }
        public virtual ApplicationUser Doctor { get; set; }
        
        public Guid ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}