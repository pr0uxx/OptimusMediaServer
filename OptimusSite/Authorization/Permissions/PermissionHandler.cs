using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace OptimusSite.Areas.Identity.Permissions
{
    public class PermissionHandler : IAuthorizationHandler
    {
        //public Task HandleAsync(AuthorizationHandlerContext context)
        //{
        //    var pendingRequirements = context.PendingRequirements.ToList();

        //    foreach (var requirement in pendingRequirements)
        //    {
        //        if (requirement is MinimumSiteLevelRequirement)
        //        {
        //        }

        //        if (requirement is ReadPermission)
        //        {
        //            if (IsOwner(context.User, context.Resource) ||
        //                IsSponsor(context.User, context.Resource))
        //            {
        //                context.Succeed(requirement);
        //            }
        //        }
        //        else if (requirement is EditPermission ||
        //                 requirement is DeletePermission)
        //        {
        //            if (IsOwner(context.User, context.Resource))
        //            {
        //                context.Succeed(requirement);
        //            }
        //        }
        //    }

        //    //TODO: Use the following if targeting a version of
        //    //.NET Framework older than 4.6:
        //    //      return Task.FromResult(0);
        //    return Task.CompletedTask;
        //}
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}