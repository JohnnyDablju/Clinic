using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public DoctorsController()
        {
        }

        public DoctorsController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public async Task<ActionResult> Index()
        {
            var role = RoleManager.FindByName("Doctor").Users.First();
            return View(await UserManager.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role.RoleId)).ToListAsync());
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterDoctorViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { FirstName = userViewModel.FirstName, LastName = userViewModel.LastName, PWZ = userViewModel.PWZ, UserName = userViewModel.Email, Email = userViewModel.Email, IsConfirmed = true };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                if (adminresult.Succeeded)
                {
                    var result = await UserManager.AddToRoleAsync(user.Id, "Doctor");
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult AssignClinics(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new AssignClinicsViewModel()
            {
                DoctorId = id,
                DoctorName = user.Name,
                ClinicsList = db.Clinics.ToList().Select(x => new SelectListItem()
                {
                    Selected = db.DoctorsToClinics.Where(y => y.DoctorId == id).Where(y => y.ClinicId == x.Id).Any(),
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignClinics(AssignClinicsViewModel model, params string[] selectedClinic)
        {
            // removing currently existing assignments
            db.DoctorsToClinics.RemoveRange(db.DoctorsToClinics.Where(y => y.DoctorId == model.DoctorId));
            if (selectedClinic != null)
            {
                foreach (var item in selectedClinic)
                {
                    db.DoctorsToClinics.Add(new Medcare.Models.DoctorToClinic() { Id = Guid.NewGuid(), DoctorId = model.DoctorId, ClinicId = new Guid(item) });
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
