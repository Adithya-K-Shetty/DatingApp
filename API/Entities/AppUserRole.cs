using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    //this acts as a join table between app user and app role
    public class AppUserRole :IdentityUserRole<int>    
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}