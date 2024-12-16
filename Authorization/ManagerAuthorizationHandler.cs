using ContosoUniversity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ContosoUniversity.Authorization{
    public class ManagerAuthorizationHandler<TResource> : AuthorizationHandler<OperationAuthorizationRequirement,
        TResource>  where TResource : AuthorizationPropertyProvider
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, TResource resource)
        {
            if(context.User ==null || resource ==null){
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != ContosoResource.ApproveOperationName &&
                requirement.Name != ContosoResource.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can approve or reject.
            if (context.User.IsInRole(ContosoResource.ContosoManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}