using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    //so when we inherit IdentityUser
    //the id defined by us will overwrite the string id property that is being inherited
    //so we explicitly specify that id has to be integer
    //by using <int>
    public class AppUser : IdentityUser<int> 
    {
        
        public DateOnly DateOfBirth {get; set;}

        public string KnowAs {get; set;}

        public DateTime Created {get; set;} = DateTime.UtcNow;

        public DateTime LastActive {get; set;} = DateTime.UtcNow;

        public string Gender {get; set;}

        public string Introduction {get; set;}

        public string LookingFor {get;set;}

        public string Interests {get; set;}

        public string City {get; set;}

        public string Country {get; set;}

        public List<Photo> Photos {get;set;} = new();

        public List<UserLike> LikedByUsers {get;set;} //users like the current user

        public List<UserLike> LikedUsers {get;set;} //users whom the current user likes

        public List<Message> MessagesSent { get; set; }

        public List<Message> MessagesReceived { get; set; }

         public ICollection<AppUserRole> UserRoles {get;set;}
    }
}