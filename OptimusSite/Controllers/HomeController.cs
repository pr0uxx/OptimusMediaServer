using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Optimus.Services.SkillService;
using OptimusSite.Models;
using System.Diagnostics;

namespace OptimusSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISkillService skillService;

        public HomeController(IdentityUser UserService, ISkillService SkillService)
        {
            skillService = SkillService;
        }

        public IActionResult Index()
        {
            ViewBag.TestString = skillService.HellowWorldString();
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