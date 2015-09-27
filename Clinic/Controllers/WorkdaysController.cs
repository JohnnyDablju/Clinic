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
        private ApplicationDbContext db = new ApplicationDbContext();

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
       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
