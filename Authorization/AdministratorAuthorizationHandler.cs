using ContosoUniversity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ContosoUniversity.Authorization{
    public class AdministratorAuthorizationHandler<TResource> : AuthorizationHandler<OperationAuthorizationRequirement,
        TResource>  where TResource : class 
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, TResource resource)
        {
            if(context.User ==null || resource ==null){
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(ContosoResource.ContosoAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}