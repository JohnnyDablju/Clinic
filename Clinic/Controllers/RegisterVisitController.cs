using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using Medcare.Models;

namespace Medcare.Controllers
{
    public class RegisterVisitController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        #region Helpers
        private ApplicationDbContext db = new ApplicationDbContext();

        private VisitProposition GetEarliestVisitByClinic(Guid ClinicId)
        {

        }

        private VisitProposition GetVisitByDoctor(DateTime when, string DoctorId, Guid? ClinicId)
        {
            var day = when.DayOfWeek;
            if (ClinicId == null)
            {
                db.Workdays.Where(w => w.DoctorId == DoctorId).Where(w => w.Day == day)
        
            }
        }
        #endregion
    }
}