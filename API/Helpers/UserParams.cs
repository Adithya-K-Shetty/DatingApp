using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class UserParams
    {
        //The UserParams class represents the parameters 
        //that can be passed by the client 
        //to specify the pagination settingsfor retrieving data

        //serParams class provides a convenient way for clients to specify 
        //the desired pagination settings, 
        //such as page number and page size
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1; //always return first page unless they specify

        //underscore for variable
        //is convention for private access specifier
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
        public string CurrentUsername { get; set; }

        public string Gender{get;set;}

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 100;

        public string OrderBy { get; set; } = "lastActive"; //sort users based on last active session

        
    }
}