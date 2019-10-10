using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QConsoleWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace QConsoleWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        public AccountController(UserManager<AppUser> userMgr,
        SignInManager<AppUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel details,
            string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(details.Username);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await signInManager.PasswordSignInAsync(
                            user, details.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.Username),
                    "Invalid user or password");
            }
            return View(details);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
