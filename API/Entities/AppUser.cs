using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }

         // [Required] :- makes the nullable property of userName false
        public string UserName { get; set; }

        public byte[] Password {get; set;}

        public byte[] PasswordSalt {get; set;}

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

        // public int GetAge(){
        //     return DateOfBirth.CalculateAge();
        // }
    }
}