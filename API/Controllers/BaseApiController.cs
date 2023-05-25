using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController] // main advantage is that automatic binding of body of the post request with that of the register method
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}