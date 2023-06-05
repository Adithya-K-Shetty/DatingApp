using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class UserParams : PaginationParams
    {
        //The UserParams class represents the parameters 
        //that can be passed by the client 
        //to specify the pagination settingsfor retrieving data

        //serParams class provides a convenient way for clients to specify 
        //the desired pagination settings, 
        //such as page number and page size
        public string CurrentUsername { get; set; }

        public string Gender{get;set;}

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 100;

        public string OrderBy { get; set; } = "lastActive"; //sort users based on last active session

        
    }
}