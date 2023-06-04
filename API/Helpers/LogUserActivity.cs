using API.Extensions;
using API.interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace API.Helpers
{
    //class used to set value for lastactive
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //resultContext gives result after
            //api execution context
            //action filter provides access to HttpContext
            //HttpContext contains users username, services etc
            var resultContext = await next();

            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            var user = await repo.GetUserByIdAsync(int.Parse(userId));

            user.LastActive = DateTime.UtcNow;

            await repo.SaveAllAsync();
        }
    }
}