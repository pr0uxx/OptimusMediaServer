using Microsoft.AspNetCore.Authorization;

namespace OptimusSite.Authorization.AuthorizationAttributes
{
    internal class AuthorizeMinimumSiteLevelAttribute : AuthorizeAttribute
    {
        private const string POLICY_PREFIX = "AuthorizeMinimumSiteLevel";

        public AuthorizeMinimumSiteLevelAttribute(int siteLevel) => SiteLevel = siteLevel;

        // Get or set the Age property by manipulating the underlying Policy property
        public int SiteLevel
        {
            get
            {
                if (int.TryParse(Policy.Substring(POLICY_PREFIX.Length), out var siteLevel))
                {
                    return siteLevel;
                }
                return default(int);
            }
            set
            {
                Policy = $"{POLICY_PREFIX}{value.ToString()}";
            }
        }
    }
}