using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Optimus.Data.Entities
{
    public class OptimusUser : IdentityUser<long>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<OptimusUser> manager)
        {
            var userIdentity = new ClaimsIdentity(await manager.GetClaimsAsync(this));

            return userIdentity;
        }

        [PersonalData]
        public string DisplayName { get; set; }

        public virtual ICollection<UserAssessedRank> UserAssessedRanks { get; set; }

        public virtual ICollection<IdentityUserClaim<long>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<long>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<long>> Tokens { get; set; }
        public virtual ICollection<IdentityUserRole<long>> UserRoles { get; set; }
    }
}