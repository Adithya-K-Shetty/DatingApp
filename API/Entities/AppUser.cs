using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }

         // [Required] :- makes the nullable property of userName false
        public string UserName { get; set; }

        public byte[] Password {get; set;}

        public byte[] PasswordSalt {get; set;}


    }
}