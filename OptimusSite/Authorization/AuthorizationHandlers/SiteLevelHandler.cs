using Microsoft.AspNetCore.Authorization;
using Optimus.Data.Enums;
using OptimusSite.Authorization.AuthorizationRequirements;
using System;
using System.Threading.Tasks;

namespace OptimusSite.Authorization.AuthorizationHandlers
{
    public class SiteLevelHandler : AuthorizationHandler<MinimumSiteLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumSiteLevelRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == CustomClaims.CustomClaimTypes.UserSiteClaim.ToString()))
            {
                return Task.CompletedTask;
            }

            var userSiteLevelString = context.User.FindFirst(x => x.Type == CustomClaims.CustomClaimTypes.UserSiteClaim.ToString()).Value;

            bool parseSuccess = Int32.TryParse(userSiteLevelString, out int siteLevel);

            if (!parseSuccess)
            {
                return Task.CompletedTask;
            }

            if (siteLevel >= requirement.SiteLevel)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}