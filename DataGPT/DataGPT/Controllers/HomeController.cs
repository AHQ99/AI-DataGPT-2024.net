using DataGPT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Auth0.AspNetCore.Authentication;
using System.Security.Claims;


namespace DataGPT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LandingPage()
        {
            return View();
        }

        
        public IActionResult DataPage()
        {
            return View();
        }

        
        public IActionResult HomePage()
        {
            return View(new
            {
                Name = User.Identity.Name,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,

            });
        }
        public IActionResult ChatGPTPage()
        {
            return View(new
            {
                Name = User.Identity.Name,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,

            });
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
