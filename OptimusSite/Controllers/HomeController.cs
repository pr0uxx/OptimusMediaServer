using Microsoft.AspNetCore.Mvc;
using Optimus.Data;
using Optimus.Services.SkillService;
using OptimusSite.Models;
using System.Diagnostics;
using System.Linq;

namespace OptimusSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISkillService skillService;
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext DbContext, ISkillService SkillService)
        {
            dbContext = DbContext;
            skillService = SkillService;
        }

        public IActionResult Index()
        {
            ViewBag.TestString = skillService.HellowWorldString();
            var emailConfirmedUsers = dbContext.Users.Where(x => x.EmailConfirmed == false);

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