using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using IdentitySample.Models;

namespace Medcare.Models
{
    public class Clinic
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name="Clinic name")]
        public string Name { get; set; }
    }

    public class DoctorToClinic
    {
        public Guid Id { get; set; }

        public string DoctorId { get; set; }
        public virtual ApplicationUser Doctor { get; set; }

        public Guid ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}