using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentitySample.Models;

namespace Medcare.Models
{
    public class VisitProposition
    {
        public DateTime StartDateTime { get; set; }
        public Clinic Clinic { get; set; }
        public ApplicationUser Doctor { get; set; }
    }
}