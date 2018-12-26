using System;

namespace Optimus.Data.Enums
{
    public class CustomClaims
    {
        [Flags]
        public enum UserSiteClaim : byte
        {
            View = 1,
            Edit = 2,
            Create = 3,
            Delete = 4,
            Moderate = 5,
            Adminsitrate = 6,
            Developer = (View | Edit | Create | Delete | Moderate | Adminsitrate)
        }

        public enum CustomClaimTypes
        {
            UserSiteClaim
        }
    }
}