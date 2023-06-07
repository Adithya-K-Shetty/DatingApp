using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;

        //key is used to both encrypt and decrypt the data
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            
            //getting just the name of the role
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //used to create signing creadential object
            //to check the authenticity of token
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            //it is used to setup 
            //configuration for the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires =  DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            //generates and handles token
            var tokenHandler = new JwtSecurityTokenHandler();
            //process the configuration
            //in the token descriptor
            //generates a new token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //converts a generated token
            //into string format
            return tokenHandler.WriteToken(token);
        }
    }
}