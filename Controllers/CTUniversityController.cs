using ContosoUniversity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.Controllers{
    public class CTUniversity : Controller{
        
        protected readonly IAuthorizationService AuthorizationService ;

        protected readonly UserManager<ContosoUser> UserManager ;

        public CTUniversity(
            IAuthorizationService authorizationService,
            UserManager<ContosoUser> userManager) : base(){
                AuthorizationService = authorizationService ;
                UserManager = userManager ;
            }
    }
}