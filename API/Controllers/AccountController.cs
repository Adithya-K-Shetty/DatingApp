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
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
          public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,IMapper mapper)
          {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;

          }
          [HttpPost("register")] //  POST : api/account/register

          // if we dont setup [Apicontroller] then we have to provide [FromBody] in the parameter of register
          public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) // query strings can be automatically binded using api controller
          {
                if(await UserExists(registerDto.UserName)) return BadRequest("Username Is Taken");

                var user = _mapper.Map<AppUser>(registerDto);

                user.UserName = registerDto.UserName.ToLower();
               
               var result = await _userManager.CreateAsync(user,registerDto.Pass);

               if(!result.Succeeded) return BadRequest(result.Errors);

               var roleResult = await _userManager.AddToRoleAsync(user,"Member"); //Adding member role at the time of registration

               if(!roleResult.Succeeded) return BadRequest(result.Errors);
                return new UserDto
                {
                    Username = user.UserName,
                    Token =await _tokenService.CreateToken(user),
                    Gender = user.Gender     
                }; // respose 
          }

          [HttpPost("login")]
          public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
          {
            var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if(user == null) return Unauthorized("Invalid Username");
            
            var result = await _userManager.CheckPasswordAsync(user,loginDto.Password);

            if(!result) return Unauthorized("Invalid Password");

             return new UserDto
                {
                    Username = user.UserName,
                    Token = await _tokenService.CreateToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    Gender = user.Gender
                };
          }
          private async Task<bool> UserExists(string username) // to check whether the user has already been registered
          {
            // AnyAsync is like a sequence
            // so basically this loops over the table
            // check whether it contains a user with username
            // same as the on passed through the function
            // basically returns a boolean value
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());

          }
        
    }

    
}