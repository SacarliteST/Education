using Education.Web.Integration;
using Microsoft.Extensions.Options;

namespace Education.Tests.PracticalModules;

public class ModuleIntegrationConfigTests
{
    private const string BasePath = "/modules/sql";
    private const string Token = "aaa.bbb.ccc";
    private static readonly Guid SessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static ModuleIntegrationConfig Build(string moduleWebOrigin) =>
        new(Options.Create(new ModuleIntegrationOptions
        {
            PlatformOrigin = "https://app.example.com/",
            ModuleWebOrigin = moduleWebOrigin,
        }));

    [Fact]
    public void BuildAuthoringUrl_BehindGateway_UsesPlatformOriginAndBasePath()
    {
        var config = Build(moduleWebOrigin: String.Empty);

        var url = config.BuildAuthoringUrl(BasePath, Token);

        Assert.Equal("https://app.example.com/modules/sql/teacher/launch#access_token=aaa.bbb.ccc", url);
    }

    [Fact]
    public void BuildAuthoringUrl_Gatewayless_UsesModuleWebOriginWithoutBasePath()
    {
        var config = Build(moduleWebOrigin: "http://localhost:5174/");

        var url = config.BuildAuthoringUrl(BasePath, Token);

        Assert.Equal("http://localhost:5174/teacher/launch#access_token=aaa.bbb.ccc", url);
    }

    [Fact]
    public void BuildAuthoringUrl_CarriesNoSessionQuery()
    {
        var url = Build(String.Empty).BuildAuthoringUrl(BasePath, Token);

        Assert.DoesNotContain("session", url);
    }

    [Theory]
    [InlineData("")]
    [InlineData("http://localhost:5174/")]
    public void BuildLaunchUrl_UnchangedBySharedBaseRefactor(string moduleWebOrigin)
    {
        var config = Build(moduleWebOrigin);
        var expected = String.IsNullOrEmpty(moduleWebOrigin)
            ? "https://app.example.com/modules/sql/launch?session=11111111-1111-1111-1111-111111111111#access_token=aaa.bbb.ccc"
            : "http://localhost:5174/launch?session=11111111-1111-1111-1111-111111111111#access_token=aaa.bbb.ccc";

        Assert.Equal(expected, config.BuildLaunchUrl(BasePath, SessionId, Token));
    }
}
