using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentitySample.Models;

namespace Medcare.Models
{
    public class Visit
    {
        public Guid Id { get; set; }
        public DateTime StartDateTime { get; set; }

        public string DoctorId { get; set; }
        public virtual ApplicationUser Doctor { get; set; }

        public string PatientId { get; set; }
        public virtual ApplicationUser Patient { get; set; }

        public Guid ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}