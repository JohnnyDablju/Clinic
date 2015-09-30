using System.Globalization;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // checking if user has been confirmed by admin
            var c1 = UserManager.Users.Where(x => x.Email == model.Email).Where(x => x.IsConfirmed == true).Any();
            // or maybe wrong data has been provided
            var c2 = UserManager.Users.Where(x => x.Email == model.Email).Any();
            if (c1 || !c2)
            {
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            // if not - information is being displayed
            else
            {
                return View("RegistrationMessage");
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterPatientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, Pesel = model.Pesel, Address = model.Address, UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var roleResult = await UserManager.AddToRoleAsync(user.Id, "Patient");
                    if (roleResult.Succeeded)
                    {
                        return View("RegistrationMessage");
                    }
                    else
                    {
                        ModelState.AddModelError("", roleResult.Errors.First());
                    }
                }
                ModelState.AddModelError("", result.Errors.First());
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePersonalData(string message)
        {
            ViewBag.StatusMessage = message;
            var model = UserManager.FindById(User.Identity.GetUserId());
            return View(new ChangePersonalDataViewModel()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PWZ = model.PWZ,
                Pesel = model.Pesel,
                Address = model.Address,
                Email = model.Email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePersonalData(ChangePersonalDataViewModel model)
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
            return RedirectToAction("ChangePersonalData", new { Message = "Data updated successfully." });
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
                return RedirectToAction("ChangePersonalData", new { Message = "Password changed successfully." });
            }
            ModelState.AddModelError("", result.Errors.First());
            return View(model);
        }

        #region Helpers
        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}