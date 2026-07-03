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

    public static void AuthenticateWithBearer(this HttpClient client, params string[] roles)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            TestAuthHandler.BearerScheme,
            String.Join(",", roles));
    }
}
