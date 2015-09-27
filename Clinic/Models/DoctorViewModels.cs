using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Medcare.Models
{
    public class CreateWorkdayViewModel
    {
        public Days Day { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }

        public IEnumerable<SelectListItem> ClinicsList { get; set; }
    }
}