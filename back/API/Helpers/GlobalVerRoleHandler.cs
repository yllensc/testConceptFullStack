using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API.Helpers;
public class GlobalVerRoleHandler : AuthorizationHandler<GlobalVerbRoleRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GlobalVerRoleHandler(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GlobalVerbRoleRequirement requirement)
    {
        //check users roles
        var roles = context.User.FindAll(c => string.Equals(c.Type, ClaimTypes.Role)).Select(c => c.Value);
        var verb = _httpContextAccessor.HttpContext?.Request.Method;
        if(string.IsNullOrEmpty(verb)) {throw new Exception($"request CAN'T be null");}
        foreach(var role in roles) {
            if (requirement.IsAllowed(role, verb)){
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        context.Fail();
        return Task.CompletedTask;
    }
}
