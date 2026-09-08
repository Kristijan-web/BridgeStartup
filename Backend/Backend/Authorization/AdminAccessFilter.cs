using Data.Access;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Backend.Authorization;

// Check the current database role as well as the validated JWT: demoted or disabled
// accounts must not retain administrator access until their token expires.
public sealed class AdminAccessFilter(ApplicationDbContext context) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
    {
        var principal = actionContext.HttpContext.User;
        if (principal.Identity?.IsAuthenticated != true ||
            !long.TryParse(principal.FindFirst("Id")?.Value, out var id))
        {
            actionContext.Result = new UnauthorizedResult();
            return;
        }
        var user = await context.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == id);
        if (user == null || user.ActivatedAt == null)
        {
            actionContext.Result = new UnauthorizedResult();
            return;
        }
        if (!string.Equals(user.Role.Name, "admin", StringComparison.OrdinalIgnoreCase))
        {
            actionContext.Result = new ForbidResult();
            return;
        }
        await next();
    }
}
