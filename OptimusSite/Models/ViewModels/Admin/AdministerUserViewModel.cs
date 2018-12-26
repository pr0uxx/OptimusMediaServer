using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace OptimusSite.Models.ViewModels.Admin
{
    public class AdministerUserViewModel
    {
        public ICollection<UserAdminsitrationData> UserList { get; set; }
        public Dictionary<string, int> UserSiteClaimSelectList { get; set; }
    }

    public class UserAdminsitrationData
    {
        public long Id { get; set; }
        public ICollection<IdentityUserClaim<long>> Claims { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}