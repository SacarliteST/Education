using System.Net.Http.Json;
using System.Text.Json;
using Education.Application.PracticalModules;

namespace Education.Web.Integration;

/// <summary>HTTP-реализация пуша сессии в бэкенд модуля с заголовком <c>X-Service-Key</c>.</summary>
internal sealed class HttpModulePushClient : IModulePushClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    /// <summary>Создаёт экземпляр класса <see cref="HttpModulePushClient"/>.</summary>
    /// <param name="httpClient">HTTP-клиент для обращения к модулю.</param>
    /// <param name="configuration">Конфигурация приложения (секреты модулей).</param>
    public HttpModulePushClient(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    /// <inheritdoc />
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

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            // 409 при пуше — сессия в модуле уже завершена/истекла (ModuleSession.Completed); итог придёт обратным вызовом.
            throw new ModuleSessionClosedException($"Сессия в модуле '{moduleSlug}' уже не активна.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ModulePushFailedException(
                $"Модуль '{moduleSlug}' вернул {(int)response.StatusCode} на пуш сессии.");
        }
    }
}
