using KRU.Data;
using KRU.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KRU.Controllers
{
    [Area("Admin")]
    [Authorize]
    // [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ClaimsPrincipal currentUser = User;

            
            if (currentUser.IsInRole(SD.Role_Manager))
            {

                return RedirectToAction("Index", "Employees", new { Area = "Manager" });
            }
            if (currentUser.IsInRole(SD.Role_Admin))
            {

                return RedirectToAction("Index", "Users", new { Area = "Admin" });
            }
            if (currentUser.IsInRole(SD.Role_Employee))
            {

                return RedirectToAction("Index", "Reports", new { Area = "Employee" });
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
