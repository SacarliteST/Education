using Education.Application.Identity;
using Education.Application.Users;
using Education.Domain.Users;

namespace Education.Tests.Users;

public sealed class ProfileClaimsTests
{
    private static readonly Guid UserId = Guid.Parse("01a150ea-3ddd-7c22-bf21-cf0c53c6a353");

    [Fact]
    public void StructuredClaims_AreUsedExactly()
    {
        var profile = ProfileClaims.FromCurrentUser(
            new Fake { Email = "a@x.test", Name = "ignored", GivenName = "Иван", FamilyName = "Петров", MiddleName = "Сергеевич" });

        Assert.True(profile.HasStructuredName);
        Assert.Equal(("Иван", "Петров", "Сергеевич"), (profile.FirstName, profile.LastName, profile.MiddleName));
        Assert.Equal("a@x.test", profile.Login);
    }

    [Fact]
    public void StructuredClaims_WithOnlyFamily_FillsBlanksWithDash()
    {
        var profile = ProfileClaims.FromCurrentUser(new Fake { Email = "a@x.test", FamilyName = "Петров" });

        Assert.Equal(("-", "Петров", "-"), (profile.FirstName, profile.LastName, profile.MiddleName));
    }

    [Theory]
    [InlineData("Иван Петров", "Иван", "Петров", "-")]
    [InlineData("Иван Петров Сергеевич", "Иван", "Петров", "Сергеевич")]
    [InlineData("Иван Петров Сергеевич Младший", "Иван", "Петров", "Сергеевич Младший")]
    [InlineData("Петров", "-", "Петров", "-")]
    [InlineData("  Иван   Петров ", "Иван", "Петров", "-")]
    public void PlainName_IsSplitAsFirstLastMiddle(string name, string first, string last, string middle)
    {
        var profile = ProfileClaims.FromCurrentUser(new Fake { Email = "a@x.test", Name = name });

        Assert.False(profile.HasStructuredName);
        Assert.Equal((first, last, middle), (profile.FirstName, profile.LastName, profile.MiddleName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ivan.petrov@x.test")]
    public void EmptyNameOrEmailAsName_UsesEmailLocalPartAsLastName(string? name)
    {
        var profile = ProfileClaims.FromCurrentUser(new Fake { Email = "ivan.petrov@x.test", Name = name });

        Assert.Equal(("-", "ivan.petrov", "-"), (profile.FirstName, profile.LastName, profile.MiddleName));
    }

    [Fact]
    public void NoEmail_LoginFallsBackToIdentityPrefix_AndLastNameToDash()
    {
        var profile = ProfileClaims.FromCurrentUser(new Fake());

        Assert.Equal("identity-01a150ea", profile.Login);
        Assert.Equal("-", profile.LastName);
    }

    [Fact]
    public void RoleResolution_PrefersAdminThenTeacher()
    {
        Assert.Equal(RoleIds.Admin, ProfileClaims.ResolveRoleId(new HashSet<string> { "Teacher", "Admin" }));
        Assert.Equal(RoleIds.Teacher, ProfileClaims.ResolveRoleId(new HashSet<string> { "Student", "Teacher" }));
        Assert.Equal(RoleIds.Student, ProfileClaims.ResolveRoleId(new HashSet<string> { "Student" }));
        Assert.Equal(RoleIds.Student, ProfileClaims.ResolveRoleId(new HashSet<string>()));
    }

    private sealed class Fake : ICurrentUser
    {
        public Guid UserId => ProfileClaimsTests.UserId;

        public string? Email { get; init; }

        public string? Name { get; init; }

        public string? GivenName { get; init; }

        public string? FamilyName { get; init; }

        public string? MiddleName { get; init; }

        public IReadOnlySet<string> Roles { get; init; } = new HashSet<string>();

        public bool IsAuthenticated => true;
    }
}
