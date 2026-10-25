using Education.Application.Users;
using Education.Contracts;
using Education.Contracts.Profile;
using Education.Domain.Users;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

/// <summary>Маршруты раздела «профиль текущего пользователя».</summary>
internal static class ProfileEndpointGroup
{
    /// <summary>Регистрирует маршруты раздела.</summary>
    /// <param name="app">Построитель маршрутов.</param>
    /// <returns>Тот же построитель маршрутов.</returns>
    public static IEndpointRouteBuilder MapProfileEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Profile.Me, async (
                IEducationUserResolver resolver,
                IUserProfileRepository repository,
                CancellationToken cancellationToken) =>
            {
                var id = await resolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
                var profile = await repository.FindProfileAsync(id, cancellationToken);
                return profile is null
                    ? Results.NotFound()
                    : Results.Ok(new MyEducationProfileResponse(
                        profile.Id,
                        profile.Login,
                        profile.FirstName,
                        profile.LastName,
                        profile.MiddleName,
                        RoleName(profile.RoleId),
                        profile.GroupName));
            })
            .WithTags("Profile")
            .WithName("GetMyEducationProfile")
            .WithSummary("Мой учебный профиль")
            .WithDescription("Логин, ФИО, роль и учебная группа текущего пользователя. Группу назначает администратор.")
            .Produces<MyEducationProfileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }

    private static string RoleName(Guid roleId) =>
        roleId == RoleIds.Admin ? "Admin" : roleId == RoleIds.Teacher ? "Teacher" : "Student";
}
