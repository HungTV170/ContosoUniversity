using ContosoUniversity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ContosoUniversity.Authorization{
    public class IsOwerAuthorizationHandler<TResource> : AuthorizationHandler<OperationAuthorizationRequirement,
        TResource>  where TResource : AuthorizationPropertyProvider
    {
        private readonly UserManager<ContosoUser> userManager;      

        public IsOwerAuthorizationHandler(UserManager<ContosoUser> manager){
            userManager = manager;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, TResource resource)
        {
            if(context.User ==null || resource ==null){
                return Task.CompletedTask;
            }

            if(requirement.Name != ContosoResource.CreateOperationName &&
                requirement.Name != ContosoResource.ReadOperationName &&
                requirement.Name != ContosoResource.DeleteOperationName &&
                requirement.Name != ContosoResource.UpdateOperationName
            ){
                return Task.CompletedTask;
            }

            if(resource.OwnerID == userManager.GetUserId(context.User )){
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}