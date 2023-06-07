using System.Security.Cryptography;
using System.Text.Json;
using API.Entities;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
            //it checks whether there is users in a database
            //if the users already present then it simply
            //returns and stops the execution
            if (await userManager.Users.AnyAsync()) return;


            //if we dont have any users
            //then we will seed the users

            //so we read the json data of users
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            //helps to handle insensitive case
            //in our json data we used pascal case

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            //deserialzing into list containing c# objects

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>
            {
                new AppRole{Name="Member"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"}
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);    
            }

            //for each user we are generating password
            foreach(var user in users)
            {
               
                user.UserName = user.UserName.ToLower();
               

                //we are adding it to entity framework tracking
                await userManager.CreateAsync(user,"Pa$$w0rd");

                await userManager.AddToRoleAsync(user,"Member");
            }

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin,"Pa$$w0rd");
            await userManager.AddToRolesAsync(admin,new[] {"Admin","Moderator"});

            //here we are adding data into the database
           
        }
    }
}