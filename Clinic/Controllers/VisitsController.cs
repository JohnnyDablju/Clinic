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
    [Authorize]
    public class VisitsController : Controller
    {
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                return View(db.Visits.OrderByDescending(v => v.StartDateTime).ToList());
            }
            else if (User.IsInRole("Doctor"))
            {
                return View(db.Visits.Where(v => v.DoctorId == id).OrderByDescending(v => v.StartDateTime).ToList());
            }
            else if (User.IsInRole("Patient"))
            {
                return View(db.Visits.Where(v => v.PatientId == id).OrderByDescending(v => v.StartDateTime).ToList());
            }
            return View();
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visit = db.Visits.Find(id);
            if (visit == null)
            {
                return HttpNotFound();
            }
            return View(visit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Visit visit = db.Visits.Find(id);
            db.Visits.Remove(visit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Helpers
        private ApplicationDbContext db = new ApplicationDbContext();
        #endregion
    }
}
