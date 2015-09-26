using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager)
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


        public ActionResult ChangePersonalData(string message)
        {
            ViewBag.StatusMessage = message;
            return View(UserManager.FindById(User.Identity.GetUserId()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePersonalData(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                user.Address = model.Address;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Pesel = model.Pesel;
                user.PWZ = model.PWZ;
                var result = UserManager.Update(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return RedirectToAction("ChangePersonalData", new { Message = result.Errors.First() });
                }
                return RedirectToAction("ChangePersonalData", new { Message = "Data updated successfully" });
            }
            return RedirectToAction("ChangePersonalData", new { Message = "Invalid data provided" });
        }


        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, await user.GenerateUserIdentityAsync(UserManager));
                }
                return RedirectToAction("ChangePersonalData", new { Message = "Password changed successfully" });
            }
            ModelState.AddModelError("", result.Errors.First());
            return View(model);
        }
    }
}