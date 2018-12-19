using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Optimus.Data.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OptimusSite.Extensions
{
    public class OptimusClaimsIdentityFactory : UserClaimsPrincipalFactory<OptimusUser>
    {
        private readonly UserManager<OptimusUser> _userManager;

        public OptimusClaimsIdentityFactory(UserManager<OptimusUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(OptimusUser user)
        {
            var principal = await base.CreateAsync(user);
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims
                    (new[] {
                        new Claim("DisplayName", user.DisplayName)
                    });
            }
            return principal;
        }
    }
}