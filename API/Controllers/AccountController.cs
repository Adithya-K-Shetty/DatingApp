using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
          public AccountController(DataContext context, ITokenService tokenService)
          {
            _tokenService = tokenService;
            _context = context;  
            
          }
          [HttpPost("register")] //  POST : api/account/register

          // if we dont setup [Apicontroller] then we have to provide [FromBody] in the parameter of register
          public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) // query strings can be automatically binded using api controller
          {
                if(await UserExists(registerDto.UserName)) return BadRequest("Username Is Taken");

                // using bascially disposes unused class
                using var hmac = new HMACSHA512();   //intializing instance to offer hash (it generates randomly generated key which can be used as password salt)

                var user = new AppUser
                {
                    UserName = registerDto.UserName.ToLower(),
                    Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), //this will compute hash of the text password
                    PasswordSalt = hmac.Key //generated new random key
                };

                _context.Users.Add(user); //just tracks new entity in memory

                await _context.SaveChangesAsync();

                return new UserDto
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                }; // respose 
          }

          [HttpPost("login")]
          public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
          {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if(user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt); //this will be in byte array format
            
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.Password[i]) return Unauthorized("Invalid Password");
            }
             return new UserDto
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
          }
          private async Task<bool> UserExists(string username) // to check whether the user has already been registered
          {
            // AnyAsync is like a sequence
            // so basically this loops over the table
            // check whether it contains a user with username
            // same as the on passed through the function
            // basically returns a boolean value
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());

          }
        
    }

    
}