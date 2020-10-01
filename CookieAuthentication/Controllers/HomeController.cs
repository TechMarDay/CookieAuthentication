using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CookieAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CookieAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var currentUser = HttpContext.User;

            ViewBag.UserName = string.Empty;
            if (currentUser.HasClaim(x => x.Type == ClaimTypes.Name))
            {
                ViewBag.UserName = currentUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
