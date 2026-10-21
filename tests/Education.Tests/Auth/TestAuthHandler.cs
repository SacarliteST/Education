using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Education.Tests.Auth;

internal sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string AuthenticationScheme = "Test";
    public const string BearerScheme = "Bearer";
    public static readonly Guid TestUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeaders))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authorization = authorizationHeaders.ToString();
        var rolesValue = GetRolesValue(authorization);
        if (rolesValue is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Необязательные заголовки подменяют личность: так тесты автосоздания входят «новым пользователем».
        var subject = Header("X-Test-Sub") is { } sub && Guid.TryParse(sub, out var parsed) ? parsed : TestUserId;
        var claims = new List<Claim>
        {
            new("sub", subject.ToString()),
            new(ClaimTypes.NameIdentifier, subject.ToString()),
            new(ClaimTypes.Name, Header("X-Test-Name") ?? "Test User"),
            new(ClaimTypes.Email, Header("X-Test-Email") ?? "test.user@example.test"),
        };
        AddIfPresent(claims, "given_name", Header("X-Test-Given-Name"));
        AddIfPresent(claims, "family_name", Header("X-Test-Family-Name"));
        AddIfPresent(claims, "middle_name", Header("X-Test-Middle-Name"));

        // ClaimTypes.Role (не короткое "role"), т.к. HttpCurrentUser.Roles ищет claim
        // строго по Jwt:RoleClaimType из appsettings.Development.json — это длинный
        // URI, под который в проде маппится JWT-claim "role" стандартным
        // JwtSecurityTokenHandler; тестовый principal должен нести тот же тип claim,
        // иначе /auth/me и всё, что читает ICurrentUser.Roles, видит пустой набор ролей.
        claims.AddRange(rolesValue
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private string? Header(string name) =>
        Request.Headers.TryGetValue(name, out var value) && !String.IsNullOrEmpty(value)
            ? Uri.UnescapeDataString(value.ToString())
            : null;

    private static void AddIfPresent(List<Claim> claims, string type, string? value)
    {
        if (!String.IsNullOrEmpty(value))
        {
            claims.Add(new Claim(type, value));
        }
    }

    private static string? GetRolesValue(string authorization)
    {
        if (authorization.StartsWith(AuthenticationScheme + " ", StringComparison.OrdinalIgnoreCase))
        {
            return authorization[(AuthenticationScheme.Length + 1)..];
        }

        if (authorization.StartsWith(BearerScheme + " ", StringComparison.OrdinalIgnoreCase))
        {
            return authorization[(BearerScheme.Length + 1)..];
        }

        return null;
    }
}
