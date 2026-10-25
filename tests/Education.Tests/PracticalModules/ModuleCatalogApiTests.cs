using System.Net;
using System.Net.Http.Json;
using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Tests.Auth;
using Education.Tests.Support;
using Education.Web.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.PracticalModules;

public sealed class ModuleCatalogApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public ModuleCatalogApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Teacher_GetsNormalizedCatalog()
    {
        var moduleId = await RegisterModuleAsync();
        var catalog = new FakeCatalogClient(_ =>
        [
            new ModuleCatalogTask("task-a", "Джойны", "Соедини две таблицы"),
            new ModuleCatalogTask("task-b", "Группировки", "Посчитай по группам"),
        ]);

        var client = CreateClientWith(catalog, EducationRoles.Teacher);
        var response = await client.GetAsync('/' + ApiRoutes.PracticalModules.ForModuleTasks(moduleId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<ModuleTaskResponse[]>();
        Assert.Equal(2, tasks!.Length);
        Assert.Equal("task-a", tasks[0].Ref);
        Assert.Equal("Джойны", tasks[0].Name);
    }

    [Fact]
    public async Task UnknownModule_Returns404()
    {
        var catalog = new FakeCatalogClient(_ => []);
        var client = CreateClientWith(catalog, EducationRoles.Teacher);

        var response = await client.GetAsync('/' + ApiRoutes.PracticalModules.ForModuleTasks(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ModuleUnavailable_Returns502()
    {
        var moduleId = await RegisterModuleAsync();
        var catalog = new FakeCatalogClient(_ =>
            throw new ModuleCatalogUnavailableException("модуль не ответил"));

        var client = CreateClientWith(catalog, EducationRoles.Teacher);
        var response = await client.GetAsync('/' + ApiRoutes.PracticalModules.ForModuleTasks(moduleId));

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
    }

    [Theory]
    [InlineData(EducationRoles.Student)]
    [InlineData(EducationRoles.Admin)]
    public async Task NonTeacher_Forbidden(string role)
    {
        var moduleId = await RegisterModuleAsync();
        var client = CreateClientWith(new FakeCatalogClient(_ => []), role);

        var response = await client.GetAsync('/' + ApiRoutes.PracticalModules.ForModuleTasks(moduleId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private Task<Guid> RegisterModuleAsync() => factory.RegisterModuleAsync(
        "{\"catalogEndpoint\":\"http://sql-module/module-integration/tasks-catalog\"}");

    private HttpClient CreateClientWith(IModuleCatalogClient catalog, string role)
    {
        var client = factory
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IModuleCatalogClient>();
                services.AddScoped(_ => catalog);
            }))
            .CreateClient();
        client.AuthenticateAs(role);
        return client;
    }

    private sealed class FakeCatalogClient(Func<PracticalModule, IReadOnlyList<ModuleCatalogTask>> handler)
        : IModuleCatalogClient
    {
        public Task<IReadOnlyList<ModuleCatalogTask>> GetTasksAsync(
            PracticalModule module, CancellationToken cancellationToken = default)
            => Task.FromResult(handler(module));
    }
}
