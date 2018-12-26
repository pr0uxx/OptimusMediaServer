using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Optimus.Data.Entities;
using OptimusSite.Authorization.AuthorizationAttributes;
using OptimusSite.Models.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Optimus.Data.Enums.CustomClaims;

namespace OptimusSite.Controllers
{
    [AuthorizeMinimumSiteLevel((int)UserSiteClaim.Adminsitrate)]
    public class AdminController : Controller
    {
        private readonly UserManager<OptimusUser> userManager;

        public AdminController(UserManager<OptimusUser> UserManager)
        {
            userManager = UserManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdministerUser()
        {
            AdministerUserViewModel viewModel = new AdministerUserViewModel
            {
                UserList = userManager.Users.Select(x => new UserAdminsitrationData()
                {
                    Id = x.Id,
                    Claims = x.Claims,
                    DisplayName = x.DisplayName,
                    Email = x.Email
                }).ToList(),
            };

            viewModel.UserSiteClaimSelectList = new Dictionary<string, int>();

            Array enumValues = Enum.GetValues(typeof(UserSiteClaim));

            foreach (UserSiteClaim i in enumValues)
            {
                viewModel.UserSiteClaimSelectList.Add(Enum.GetName(typeof(UserSiteClaim), i), (int)i);
            }

            //foreach (var i in enumValues)
            //{
            //    viewModel.UserSiteClaimSelectList.Add(Enum.GetName(typeof(UserSiteClaim), i), byte(i))
            //}

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddClaimsToUsers([FromBody]AddUserClaimModel claimModel)
        {
            var result = new JsonResultList();

            var user = userManager.Users.FirstOrDefault(x => x.Id == long.Parse(claimModel.UserId));

            var addResult = await userManager.AddClaimAsync(user, new Claim(claimModel.ClaimType, claimModel.ClaimValue));

            var r = new JsonResultMetaData()
            {
                Id = claimModel.UserId.ToString()
            };

            if (addResult.Succeeded)
            {
                r.Success = true;
            }
            else
            {
                foreach (var e in addResult.Errors)
                {
                    r.Errors.Add(string.Format("{0}::{1}", e.Code, e.Description));
                }
            }

            result.MetaData.Add(r);

            return Json(result);
        }

        public class JsonResultList
        {
            public List<JsonResultMetaData> MetaData { get; set; } = new List<JsonResultMetaData>();
        }

        public class JsonResultMetaData
        {
            public string Id { get; set; }
            public bool Success { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }

        public class AddUserClaimModel
        {
            public string UserId { get; set; }
            public string ClaimType { get; set; }
            public string ClaimValue { get; set; }
        }
    }
}