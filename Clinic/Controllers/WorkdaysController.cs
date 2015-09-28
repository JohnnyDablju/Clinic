using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using Medcare.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Medcare.Controllers
{
    [Authorize(Roles="Doctor")]
    public class WorkdaysController : Controller
    {
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            var workdays = db.Workdays.Where(w => w.DoctorId == id);
            return View(workdays.ToList());
        }

        public ActionResult Create()
        {
            var id = User.Identity.GetUserId();
            ViewBag.Clinics = new SelectList(db.DoctorsToClinics.Where(dc => dc.DoctorId == id).Select(dc => dc.Clinic).ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateWorkdayViewModel model, params string[] ClinicId)
        {
            // converting time strings
            string[] hhmm = model.StartHour.Split(':');
            var startTime = new DateTime(2000, 1, 1, Convert.ToInt32(hhmm[0]), Convert.ToInt32(hhmm[1]), 0);
            hhmm = model.EndHour.Split(':');
            var endTime = new DateTime(2000, 1, 1, Convert.ToInt32(hhmm[0]), Convert.ToInt32(hhmm[1]), 0);
            // checking if new plan is correct
            var id = User.Identity.GetUserId();
            // getting plans for chosen day
            var existingPlans = db.Workdays.Where(w => w.DoctorId == id).Where(w => w.Day == model.Day).ToList();
            bool flag = true;
            foreach (var plan in existingPlans)
            {
                if (startTime < plan.EndHour && endTime > plan.StartHour)
                {
                    flag = false;
                    break;
                }
            }
            if (flag && ClinicId != null)
            {
                db.Workdays.Add(new Workday(){ Id = Guid.NewGuid(), Day = model.Day, StartHour = startTime, EndHour = endTime, DoctorId = id, ClinicId = new Guid(ClinicId[0]) });
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                if (!flag) ModelState.AddModelError("", "Provided plan overlaps existing ones.");
                // not the best solution ever, but quick one
                ViewBag.Clinics = new SelectList(db.DoctorsToClinics.Where(dc => dc.DoctorId == id).Select(dc => dc.Clinic).ToList(), "Id", "Name");
                return View(model);
            }
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var workday = db.Workdays.Find(id);
            if (workday == null)
            {
                return HttpNotFound();
            }
            return View(workday);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var workday = db.Workdays.Find(id);
            db.Workdays.Remove(workday);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Helpers
        private ApplicationDbContext db = new ApplicationDbContext();
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
