using System.Net.Http.Json;
using System.Text.Json;
using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;

namespace Education.Web.Integration;

/// <summary>
/// HTTP-реализация клиента каталога заданий модуля. Читает адрес из
/// <c>configuration.catalogEndpoint</c>, добавляет заголовок <c>X-Service-Key</c>
/// (секрет из конфига <c>PracticalModules:&lt;slug&gt;:ServiceKey</c>), нормализует ответ.
/// </summary>
internal sealed class HttpModuleCatalogClient(HttpClient httpClient, IConfiguration configuration)
    : IModuleCatalogClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<ModuleCatalogTask>> GetTasksAsync(
        PracticalModule module,
        CancellationToken cancellationToken = default)
    {
        var endpoint = ReadCatalogEndpoint(module);

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var serviceKey = configuration[$"PracticalModules:{module.Slug}:ServiceKey"];
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
            throw new ModuleCatalogUnavailableException(
                $"Модуль '{module.Slug}' не ответил на запрос каталога заданий.");
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ModuleCatalogUnavailableException(
                $"Модуль '{module.Slug}' вернул {(int)response.StatusCode} на запрос каталога заданий.");
        }

        List<ModuleCatalogTask>? tasks;
        try
        {
            tasks = await response.Content.ReadFromJsonAsync<List<ModuleCatalogTask>>(JsonOptions, cancellationToken);
        }
        catch (JsonException)
        {
            throw new ModuleCatalogUnavailableException(
                $"Модуль '{module.Slug}' вернул нераспознаваемый ответ на запрос каталога заданий.");
        }

        return tasks ?? [];
    }

    private static string ReadCatalogEndpoint(PracticalModule module)
    {
        try
        {
            using var document = JsonDocument.Parse(module.Configuration);
            if (document.RootElement.TryGetProperty("catalogEndpoint", out var endpoint)
                && endpoint.ValueKind == JsonValueKind.String
                && !String.IsNullOrWhiteSpace(endpoint.GetString()))
            {
                return endpoint.GetString()!;
            }
        }
        catch (JsonException)
        {
            // ниже — общее сообщение об отсутствии адреса
        }

        throw new ModuleCatalogUnavailableException(
            $"У модуля '{module.Slug}' не задан configuration.catalogEndpoint.");
    }
}
