using System.Net.Http.Json;
using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.Support;

/// <summary>Общие помощники тестов внешних практических модулей.</summary>
public static class ModuleTestExtensions
{
    /// <summary>Конфигурация модуля с адресом приёма сессий — достаточна для запуска практик.</summary>
    public const string SessionsConfiguration = "{\"sessionsEndpoint\":\"http://sql-module/module-integration/sessions\"}";

    /// <summary>Хост, где пуш сессий и обмен токена заменены фейками.</summary>
    public static WebApplicationFactory<Program> WithModuleFakes(
        this TestWebApplicationFactory factory, IModulePushClient push)
    {
        return factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IModulePushClient>();
            services.RemoveAll<ITokenExchangeClient>();
            services.AddSingleton(push);
            services.AddSingleton<ITokenExchangeClient>(new FakeTokenExchangeClient());
        }));
    }

    /// <summary>Регистрирует модуль через API от имени администратора (slug уникален) и возвращает его идентификатор.</summary>
    /// <param name="factory">Тестовый хост.</param>
    /// <param name="configuration">JSON конфигурации модуля.</param>
    /// <param name="enabled">Если <see langword="false"/>, модуль сразу отключается.</param>
    public static async Task<Guid> RegisterModuleAsync(
        this TestWebApplicationFactory factory, string? configuration = SessionsConfiguration, bool enabled = true)
    {
        var admin = factory.CreateClient();
        admin.AuthenticateAs(EducationRoles.Admin);
        var slug = "sql-" + Guid.NewGuid().ToString("N")[..8];

        var response = await admin.PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ModulesList,
            new CreatePracticalModuleRequest(
                slug, "SQL", "desc", "SQL_SIMULATOR", "/modules/sql", "sql-module-api", configuration));
        response.EnsureSuccessStatusCode();
        var moduleId = (await response.Content.ReadFromJsonAsync<PracticalModuleResponse>())!.Id;

        if (!enabled)
        {
            var disabled = await admin.PutAsJsonAsync(
                '/' + ApiRoutes.PracticalModules.ForModule(moduleId),
                new UpdatePracticalModuleRequest(
                    "SQL", "desc", "SQL_SIMULATOR", "/modules/sql", "sql-module-api", configuration ?? "{}", IsEnabled: false));
            disabled.EnsureSuccessStatusCode();
        }

        return moduleId;
    }
}
