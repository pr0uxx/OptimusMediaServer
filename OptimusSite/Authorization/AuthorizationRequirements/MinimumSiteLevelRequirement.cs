using Microsoft.AspNetCore.Authorization;
using System;
using static Optimus.Data.Enums.CustomClaims;

namespace OptimusSite.Authorization.AuthorizationRequirements
{
    public class MinimumSiteLevelRequirement : IAuthorizationRequirement
    {
        public int SiteLevel { get; private set; }

        public MinimumSiteLevelRequirement(string siteLevel)
        {
            bool success = Int32.TryParse(siteLevel, out int result);

            if (success)
            {
                SiteLevel = result;
            }
            else
            {
                var eSuccess = Enum.TryParse(typeof(UserSiteClaim), siteLevel, out object result2);

                if (eSuccess)
                {
                    if (result2.GetType() == typeof(int))
                    {
                        SiteLevel = (int)result2;
                    }
                }
                else
                {
                    throw new FormatException("String value should convert to UserSiteClaim enum integer");
                }
            }
        }

        public MinimumSiteLevelRequirement(int siteLevel)
        {
            SiteLevel = siteLevel;
        }
    }
}