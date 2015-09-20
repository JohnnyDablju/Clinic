using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clinic.Models;
using IdentitySample.Models;

namespace Clinic.Controllers
{
    public class DoctorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Doctor
        public ActionResult Index()
        {
            return View(db.DoctorModels.ToList());
        }

        // GET: Doctor/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorModel doctorModel = db.DoctorModels.Find(id);
            if (doctorModel == null)
            {
                return HttpNotFound();
            }
            return View(doctorModel);
        }

        // GET: Doctor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Doctor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,PWZ,Email,Password")] DoctorModel doctorModel)
        {
            if (ModelState.IsValid)
            {
                db.DoctorModels.Add(doctorModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctorModel);
        }

        // GET: Doctor/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorModel doctorModel = db.DoctorModels.Find(id);
            if (doctorModel == null)
            {
                return HttpNotFound();
            }
            return View(doctorModel);
        }

        // POST: Doctor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,PWZ,Email,Password")] DoctorModel doctorModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctorModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctorModel);
        }

        // GET: Doctor/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorModel doctorModel = db.DoctorModels.Find(id);
            if (doctorModel == null)
            {
                return HttpNotFound();
            }
            return View(doctorModel);
        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DoctorModel doctorModel = db.DoctorModels.Find(id);
            db.DoctorModels.Remove(doctorModel);
            db.SaveChanges();
            return RedirectToAction("Index");
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
