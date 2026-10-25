using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Education.Application.PracticalModules;
using Microsoft.Extensions.Options;

namespace Education.Web.Integration;

/// <summary>
/// Обменивает Bearer-токен текущего пользователя (из входящего запроса) на токен под
/// audience модуля через IdentityService Token Exchange. <c>session_id</c> кладётся в
/// токен, только если сессия передана.
/// </summary>
internal sealed class HttpTokenExchangeClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor,
    IOptions<ModuleIntegrationOptions> options) : ITokenExchangeClient
{
    private const string GrantType = "urn:ietf:params:oauth:grant-type:token-exchange";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ModuleIntegrationOptions options = options.Value;

    public async Task<ExchangedToken> ExchangeAsync(
        string audience,
        Guid? sessionId = null,
        DateTimeOffset? sessionExpiresAt = null,
        CancellationToken cancellationToken = default)
    {
        var subjectToken = ReadSubjectToken();

        using var request = new HttpRequestMessage(HttpMethod.Post, options.TokenExchangeUrl)
        {
            Content = JsonContent.Create(
                new
                {
                    grantType = GrantType,
                    subjectToken,
                    audience,
                    sessionId = sessionId?.ToString(),
                    sessionExpiresAt,
                },
                options: JsonOptions),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(
                $"{options.IdentityClientId}:{options.IdentityClientSecret}")));

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            throw new ModulePushFailedException("IdentityService не ответил на обмен токена.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ModulePushFailedException(
                $"IdentityService вернул {(int)response.StatusCode} на обмен токена.");
        }

        var body = await response.Content.ReadFromJsonAsync<TokenExchangeBody>(JsonOptions, cancellationToken);
        if (body is null || String.IsNullOrEmpty(body.AccessToken))
        {
            throw new ModulePushFailedException("IdentityService вернул пустой ответ на обмен токена.");
        }

        return new ExchangedToken(body.AccessToken, body.ExpiresIn);
    }

    private string ReadSubjectToken()
    {
        var header = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!String.IsNullOrEmpty(header) && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return header["Bearer ".Length..].Trim();
        }

        throw new ModulePushFailedException("Во входящем запросе нет Bearer-токена для обмена.");
    }

    private sealed record TokenExchangeBody(string AccessToken, int ExpiresIn);
}
