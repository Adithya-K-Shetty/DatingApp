using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user){
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        public static string GetUsername(this ClaimsPrincipal user)
        {
            //retrieves the first claim of type ClaimTypes.NameIdentifier
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}