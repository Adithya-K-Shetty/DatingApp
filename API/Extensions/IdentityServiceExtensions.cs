using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration configuration)
        {

            //This line configures the authentication scheme to use JSON Web Tokens (JWT) 
            //bearer authentication.
            //It sets the default authentication scheme to JwtBearer.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>{
                //The TokenValidationParameters class allows you to 
                //define various parameters for validating and decoding JWT tokens.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // JWT token's signature will be validated against the 
                    //provided issuer signing key.
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                };
            });
            return services;
        }
    }
}