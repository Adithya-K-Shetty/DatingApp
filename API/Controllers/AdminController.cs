using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            
        }

        [Authorize(Policy ="RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            //proejecting to user roles table and there by getting data from roles table
            //here we defined anonymus object
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(
                        r => r.Role.Name
                    ).ToList()
                })
                .ToListAsync();

                return Ok(users);
        }

        [Authorize(Policy ="RequireAdminRole")]
        [HttpPost("edit-roles/{username}")] //username is the name of the user whoes role we wish to edit
        public async Task<ActionResult>  EditRoles(string username, [FromQuery]string roles)
        {
            if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one row");


            var selectedRoles = roles.Split(",").ToArray(); //converting comma separated list of string to array

            var user = await _userManager.FindByNameAsync(username); //we can even get it from claims

            if(user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles));

            if(!result.Succeeded) BadRequest("Failed to add roles");

            //removing the user from role which he was alredy in
            //and if the role is not represent inside the selectedRoles list
            //then that role will be deleted
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles)); 

            if(!result.Succeeded) return BadRequest("Failed to remove from roles");
            
            return Ok(await _userManager.GetRolesAsync(user));     
        }



        [Authorize(Policy ="ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins Or Moderators Can See This");
        }
    }
}