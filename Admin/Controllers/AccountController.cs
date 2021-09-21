using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Sepidar.Admin.Models;
using Sepidar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sepidar.Admin.Controllers
{
    public class AccountController : GeneralController
    {
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var url = $"{BaseUrl}/Account/Login";
            var result = HttpCaller.PostAndGetCookie<LoginResponseModel>(url, model, Configuration["Security:AuthenticationCookieName"], Request.Headers);

            if (result.Status != "success")
            {
                ModelState.AddModelError("", result.Message.ToString());
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim("UserId", result.Result.UserId.ToString()),
                new Claim("IsAdmin", result.Result.IsAdmin.ToString()),
                new Claim("IsEndUser", result.Result.IsEndUser.ToString()),
                new Claim("Username", model.Username),
                new Claim("Password", model.Password)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.Now.AddYears(1)
            });

            if (string.IsNullOrEmpty(model.ReturnUrl))
                return RedirectToAction("List", "Home");

            return Redirect(model.ReturnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var cookieName = Configuration["Security:AuthenticationCookieName"];
            Response.Cookies.Delete(cookieName);

            await HttpContext.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }
    }
}
