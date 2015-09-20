using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clinic.Models
{
    public class DoctorModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PWZ { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}