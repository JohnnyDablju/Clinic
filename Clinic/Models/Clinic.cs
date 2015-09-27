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

    public class DoctorToClinic
    {
        public Guid Id { get; set; }
        public string DoctorId { get; set; }
        public Guid ClinicId { get; set; }
    }
}