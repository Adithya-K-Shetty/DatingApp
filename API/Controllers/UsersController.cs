using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController //Inheritence
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)  // dependency injection
        {
            _context = context;
            
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users =await _context.Users.ToListAsync();
            return users;

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GeUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }
    }
}