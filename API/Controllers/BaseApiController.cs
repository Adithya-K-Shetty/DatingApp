using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //it enables built-in API-related behaviors
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController] // main advantage is that automatic binding of body of the post request with that of the register method
    [Route("api/[controller]")]
    //gains access to various properties and methods 
    //that facilitate handling HTTP requests and generating responses 
    public class BaseApiController : ControllerBase
    {
        
    }
}