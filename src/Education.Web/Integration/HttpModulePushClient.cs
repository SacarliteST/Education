using System.Net.Http.Json;
using System.Text.Json;
using Education.Application.PracticalModules;

namespace Education.Web.Integration;

/// <summary>HTTP-реализация пуша сессии в бэкенд модуля с заголовком <c>X-Service-Key</c>.</summary>
internal sealed class HttpModulePushClient(HttpClient httpClient, IConfiguration configuration) : IModulePushClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task PushAsync(
        string sessionsEndpoint,
        string moduleSlug,
        ModuleSessionPush push,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, sessionsEndpoint)
        {
            Content = JsonContent.Create(push, options: JsonOptions),
        };

        var serviceKey = configuration[$"PracticalModules:{moduleSlug}:ServiceKey"];
        if (!String.IsNullOrEmpty(serviceKey))
        {
            request.Headers.Add("X-Service-Key", serviceKey);
        }

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            throw new ModulePushFailedException($"Модуль '{moduleSlug}' не принял пуш сессии (нет ответа).");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ModulePushFailedException(
                $"Модуль '{moduleSlug}' вернул {(int)response.StatusCode} на пуш сессии.");
        }
    }
}
