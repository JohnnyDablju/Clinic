using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using Medcare.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Medcare.Controllers
{
    [Authorize(Roles="Patient")]
    public class ScheduleVisitController : Controller
    {
        public ActionResult Index(string message)
        {
            ViewBag.StatusMessage = message;
            ViewBag.Doctors = new SelectList(db.Workdays.Select(w => w.Doctor).Distinct().ToList(), "Id", "Name");
            ViewBag.Clinics = new SelectList(db.Workdays.Select(w => w.Clinic).Distinct().ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexOne(params string[] ClinicId)
        {
            if (ClinicId[0] == "")
            {
                return RedirectToAction("Index", new { message = "Clinic was not selected." });
            }
            var model = GetVisitByClinic(DateTime.Now, new Guid(ClinicId[0]));
            return RedirectToAction("Proposition", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexTwo(params string[] DoctorId)
        {
            if (DoctorId[0] == "")
            {
                return RedirectToAction("Index", new { message = "Doctor was not selected." });
            }
            var model = GetVisitByDoctor(DateTime.Now, DoctorId[0], null);
            return RedirectToAction("Proposition", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexThree(params string[] ClinicId)
        {
            if (ClinicId[0] == "")
            {
                return RedirectToAction("Index", new { message = "Clinic was not selected." });
            }
            return RedirectToAction("Assistant", new { ClinicId = ClinicId[0] });
        }

        public ActionResult Proposition(VisitProposition model)
        {
            var doctor = UserManager.FindById(model.DoctorId);
            var clinic = db.Clinics.Find(model.ClinicId);
            return View(new VisitPropositionViewModel()
            {
                ClinicId = model.ClinicId,
                DoctorId = model.DoctorId,
                StartDateTime = model.StartDateTime,
                Clinic = clinic,
                Doctor = doctor,
                Date = model.StartDateTime,
                Time = model.StartDateTime,
                Day = model.StartDateTime.DayOfWeek
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptProposition(VisitPropositionViewModel model)
        {
            Visit visit = new Visit()
            {
                Id = Guid.NewGuid(),
                ClinicId = model.ClinicId,
                DoctorId = model.DoctorId,
                PatientId = User.Identity.GetUserId(),
                StartDateTime = model.StartDateTime
            };
            db.Visits.Add(visit);
            db.SaveChanges();
            return RedirectToAction("Index", "Visits");
        }

        public ActionResult Assistant(string ClinicId)
        {
            ViewBag.Doctors = new SelectList(db.Workdays.Where(w => w.ClinicId == new Guid(ClinicId)).Select(w => w.Doctor).Distinct().ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assistant(ScheduleVisitAdvancedViewModel model, params string[] DoctorId)
        {
            ViewBag.Doctors = new SelectList(db.Workdays.Where(w => w.ClinicId == model.ClinicId).Select(w => w.Doctor).Distinct().ToList(), "Id", "Name");
            string[] date = model.Date.Split('-');
            try
            {
                if (DoctorId[0] == "")
                {
                    ModelState.AddModelError("", "Doctor was not selected.");
                }
                DateTime when = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0]));
                if (DateTime.Now.Date > when.Date)
                {
                    ModelState.AddModelError("", "Date cannot come from the past.");
                }

                if (ModelState.IsValid && ModelState.Values.Where(v => v.Errors.Count != 0).Count() == 1)
                {
                    var propositionModel = GetVisitByDoctor(when, DoctorId[0], model.ClinicId);
                    return RedirectToAction("Proposition", propositionModel);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ModelState.AddModelError("", "Date doesn't exist.");
            }

            return View(model); 
        }

        #region Helpers
        private VisitProposition GetVisitByClinic(DateTime when, Guid ClinicId)
        {
            var ids = db.Workdays.Where(w => w.ClinicId == ClinicId).Select(w => w.DoctorId).Distinct().ToList();
            List<VisitProposition> propositions = new List<VisitProposition>();
            foreach (var id in ids)
            {
                propositions.Add(GetVisitByDoctor(when, id, ClinicId));
            }
            var earliest = propositions[0];
            for (int i = 1; i < propositions.Count; i++)
            {
                if (propositions[i].StartDateTime < earliest.StartDateTime)
                {
                    earliest = propositions[i];
                }
            }
            return earliest;
        }

        private VisitProposition GetVisitByDoctor(DateTime when, string DoctorId, Guid? ClinicId)
        {
            VisitProposition proposition = null;
            do
            {
                var day = when.DayOfWeek;
                List<Workday> workday;
                // in case we're interested in particular doctor only
                if (ClinicId == null)
                {
                    workday = db.Workdays.Where(w => w.DoctorId == DoctorId).Where(w => w.Day == day).ToList();
                }
                // in case we're interested in doctor within particular clinic
                else
                {
                    workday = db.Workdays.Where(w => w.DoctorId == DoctorId).Where(w => w.ClinicId == ClinicId).Where(w => w.Day == day).ToList();
                }

                if (workday.Count == 0)
                {
                    when = when.AddHours(24 - when.Hour + 5);
                    continue;
                }
                else
                {
                    // getting hours of visits for the day
                    var visits = db.Visits.Where(v => v.DoctorId == DoctorId).ToList().Where(v => DateTime.Compare(v.StartDateTime.Date, when.Date) == 0).Select(v => v.StartDateTime).OrderBy(t => t.TimeOfDay).ToList();
                    foreach (var plan in workday)
                    {
                        // preparing when, to make sure that we schedule to visit which WILL happen
                        if (when.TimeOfDay > plan.StartHour.TimeOfDay)
                        {
                            if (when.Minute > 30)
                            {
                                when = when.AddHours(1);
                                when = when.AddMinutes(when.Minute);
                            }
                            else
                            {
                                when = when.AddMinutes(30 - when.Minute);
                            }
                            plan.StartHour.AddHours(when.Hour - plan.StartHour.Hour);
                            plan.StartHour.AddMinutes(when.Minute - plan.StartHour.Minute);
                        }
                        // looking for 30 minutes gap
                        for (DateTime i = plan.StartHour; i < plan.EndHour; i = i.AddMinutes(30))
                        {
                            bool flag = false;
                            foreach (var visit in visits)
                            {
                                // proposed time overlaps with existing visit
                                if (i.TimeOfDay == visit.TimeOfDay)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            // no visit on proposed time
                            if (!flag)
                            {
                                var dt = new DateTime(when.Year, when.Month, when.Day, i.Hour, i.Minute, 0);
                                proposition = new VisitProposition() { DoctorId = DoctorId, ClinicId = plan.ClinicId, StartDateTime = dt };
                                return proposition;
                            }
                        }
                    }
                    // all visits are scheduled already, let's move to another day
                    when = when.AddHours(24 - when.Hour + 5);
                }
            }
            while (true);
        }

        private ApplicationDbContext db = new ApplicationDbContext();
        public ScheduleVisitController()
        {
        }
        public ScheduleVisitController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion
    }
}