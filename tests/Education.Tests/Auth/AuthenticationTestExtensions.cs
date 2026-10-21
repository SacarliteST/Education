using System.Net.Http.Headers;

namespace Education.Tests.Auth;

internal static class AuthenticationTestExtensions
{
    public static void AuthenticateAs(this HttpClient client, params string[] roles)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            TestAuthHandler.AuthenticationScheme,
            String.Join(",", roles));
    }

    /// <summary>Аутентификация от имени произвольного пользователя Identity (значения передаются в URL-кодировке).</summary>
    public static void AuthenticateAsIdentity(
        this HttpClient client,
        Guid userId,
        string[] roles,
        string? email = null,
        string? name = null,
        string? givenName = null,
        string? familyName = null,
        string? middleName = null)
    {
        client.AuthenticateAs(roles);
        client.DefaultRequestHeaders.Remove("X-Test-Sub");
        client.DefaultRequestHeaders.Add("X-Test-Sub", userId.ToString());
        foreach (var (header, value) in new[]
                 {
                     ("X-Test-Email", email), ("X-Test-Name", name), ("X-Test-Given-Name", givenName),
                     ("X-Test-Family-Name", familyName), ("X-Test-Middle-Name", middleName),
                 })
        {
            client.DefaultRequestHeaders.Remove(header);
            if (value is not null)
            {
                client.DefaultRequestHeaders.Add(header, Uri.EscapeDataString(value));
            }
        }
    }

    public static void AuthenticateWithBearer(this HttpClient client, params string[] roles)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            TestAuthHandler.BearerScheme,
            String.Join(",", roles));
    }
}
