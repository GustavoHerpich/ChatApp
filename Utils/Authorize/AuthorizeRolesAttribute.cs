using Chat.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Utils.Authorize;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params Roles[] roles)
    {
        Roles = string.Join(",", roles.Select(r => r.ToString()));
    }
}
