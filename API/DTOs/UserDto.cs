using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    //All these data gets stored at local persistent storage
    public class UserDto
    {
        public string Username{get; set;}
        public string Token{get; set;}

        public string PhotoUrl {get; set;}

        public string Gender{get;set;}
    }
}