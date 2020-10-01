using CookieAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CookieAuthentication.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
            {
                return View();
            }

            ClaimsIdentity identity = null;
            bool isAuthenticated = false;
            UserModel user = null;

            //Chỗ này dự án thực tế sẽ query check trong database
            if (userName == "AdminTest" && password == "1234@")
                user = new UserModel
                {
                    UserName = "AdminTest",
                    PassWord = "1234@",
                    Role = "Admin" //Demo thử với quyền admin
                };
            if (user != null)
            {
                if (user.Role == "Admin")
                {
                    identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("userName", user.UserName),
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                    isAuthenticated = true;
                }
            }


            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Redirect("Home/Index");
            }

            return View();
        }
    }
}