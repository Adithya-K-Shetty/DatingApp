using System.Security.Cryptography;
using System.Text.Json;
using API.Entities;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            //it checks whether there is users in a database
            //if the users already present then it simply
            //returns and stops the execution
            if (await context.Users.AnyAsync()) return;


            //if we dont have any users
            //then we will seed the users

            //so we read the json data of users
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            //helps to handle insensitive case
            //in our json data we used pascal case

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            //deserialzing into list containing c# objects

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            //for each user we are generating password
            foreach(var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.Password = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                //we are adding it to entity framework tracking
                context.Users.Add(user);
            }

            //here we are adding data into the database
            await context.SaveChangesAsync();
        }
    }
}