using System.Security.Claims;
using Education.Application.Identity;

namespace Education.Web.Identity;

public sealed class HttpCurrentUser(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : ICurrentUser
{
    private readonly ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;

    public Guid UserId
    {
        get
        {
            var userIdClaimType = configuration["Jwt:UserIdClaimType"] ?? ClaimTypes.NameIdentifier;
            var value = user?.FindFirstValue(userIdClaimType)
                ?? user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue("sub");

            return Guid.TryParse(value, out var userId) ? userId : Guid.Empty;
        }
    }

    public string? Email => user?.FindFirstValue(ClaimTypes.Email) ?? user?.FindFirstValue("email");

    public string? Name => user?.Identity?.Name
        ?? user?.FindFirstValue(ClaimTypes.Name)
        ?? user?.FindFirstValue("name");

    public IReadOnlySet<string> Roles
    {
        get
        {
            var roleClaimType = configuration["Jwt:RoleClaimType"] ?? ClaimTypes.Role;
            var roles = user?.FindAll(roleClaimType).Select(claim => claim.Value)
                ?? Enumerable.Empty<string>();

            return roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }

    public bool IsAuthenticated => user?.Identity?.IsAuthenticated == true;
}
