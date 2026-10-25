using System.Security.Claims;
using Education.Application.Identity;

namespace Education.Web.Identity;

/// <summary>
/// Извлекает сведения о текущем пользователе из HTTP-контекста и JWT-claims.
/// </summary>
public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IConfiguration configuration;
    private readonly ClaimsPrincipal? user;

    /// <summary>Создаёт экземпляр класса <see cref="HttpCurrentUser"/>.</summary>
    /// <param name="httpContextAccessor">Доступ к текущему HTTP-контексту.</param>
    /// <param name="configuration">Конфигурация типов claims для JWT.</param>
    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        this.configuration = configuration;
        user = httpContextAccessor.HttpContext?.User;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public string? Email => user?.FindFirstValue(ClaimTypes.Email) ?? user?.FindFirstValue("email");

    /// <inheritdoc />
    public string? Name => user?.Identity?.Name
        ?? user?.FindFirstValue(ClaimTypes.Name)
        ?? user?.FindFirstValue("name");

    /// <inheritdoc />
    public string? GivenName => user?.FindFirstValue("given_name") ?? user?.FindFirstValue(ClaimTypes.GivenName);

    /// <inheritdoc />
    public string? FamilyName => user?.FindFirstValue("family_name") ?? user?.FindFirstValue(ClaimTypes.Surname);

    /// <inheritdoc />
    public string? MiddleName => user?.FindFirstValue("middle_name");

    /// <inheritdoc />
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

    /// <inheritdoc />
    public bool IsAuthenticated => user?.Identity?.IsAuthenticated == true;
}

