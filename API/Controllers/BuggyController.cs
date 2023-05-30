using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
            
        }

        [Authorize] //because of authorize error will be generated
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }
        [HttpGet("not-found")] //because of wrong user id error will be generated
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);

            if(thing == null) return NotFound();

            return thing;
        }
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
           
                var thing = _context.Users.Find(-1);

                var thingToReturn = thing.ToString(); //because of converting null to string exception will be raised

                return thingToReturn;
        }
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a goods request");
        }
    }
}