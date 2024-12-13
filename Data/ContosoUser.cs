
using Microsoft.AspNetCore.Identity;

namespace ContosoUniversity.Data{
    public class ContosoUser : IdentityUser {
        [PersonalData]
        public string? DOB {get; set;}
    }
}