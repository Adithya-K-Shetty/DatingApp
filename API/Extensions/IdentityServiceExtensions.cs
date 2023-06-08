using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
            services.AddIdentityCore<AppUser>(opt => 
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();  //job is to create all of the table to identity in database

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
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>{
                        //access-token is the name used by signalR at the client side   
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;  
                    }
                };
            });

            services.AddAuthorization(opt => {
                opt.AddPolicy("RequireAdminRole",policy => policy.RequireRole("Admin"));
                 opt.AddPolicy("ModeratePhotoRole",policy => policy.RequireRole("Admin","Moderator"));
            });
            return services;
        }
    }
}