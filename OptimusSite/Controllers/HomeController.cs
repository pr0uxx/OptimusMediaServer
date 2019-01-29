using Microsoft.AspNetCore.Mvc;
using Optimus.Data;
using Optimus.Services.SkillService;
using OptimusSite.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Net.Http.Headers;

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

        [HttpGet]
        public IActionResult GetTestVideo(string filepath)
        {
            filepath = filepath.TrimStart('\\');

            return File(System.IO.File.OpenRead(filepath), "video/mp4", true);

            //var fs = System.IO.File.Open(filepath,
            //    FileMode.Open, FileAccess.Read, FileShare.Read);


            //if (fs != null)
            //{
            //    return new FileStreamResult(fs, new MediaTypeHeaderValue("video/mp4"));
            //}

            //return BadRequest();
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