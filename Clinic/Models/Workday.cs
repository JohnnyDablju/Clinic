using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentitySample.Models;

namespace Medcare.Models
{
    public enum Days { Monday, Tuesday, Wendesday, Thursday, Friday }
    public class Workday
    {
        public Guid Id { get; set; }
        public Days Day { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }

        public string DoctorId { get; set; }
        public virtual ApplicationUser Doctor { get; set; }

        public Guid ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}