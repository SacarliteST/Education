using Education.Application.AdminProfiles;
using Education.Contracts;
using Education.Contracts.AdminProfiles;
using Education.Contracts.Courses;
using Education.Contracts.Practicals;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class AdminProfilesEndpointGroup
{
    public static IEndpointRouteBuilder MapAdminProfilesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.AdminProfiles.ProfilesList, async (
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetProfilesAsync(cancellationToken)).Select(profile => profile.ToResponse())))
            .WithTags("AdminProfiles")
            .WithName("GetAdminProfiles")
            .WithSummary("Получение профилей пользователей")
            .WithDescription("Возвращает связанные профили пользователей для администрирования.")
            .Produces<IEnumerable<AdminProfileResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapPost(ApiRoutes.AdminProfiles.ProfilesList, async (
                CreateAdminProfileRequest request,
                IValidator<CreateAdminProfileRequest> validator,
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                try
                {
                    var profile = await service.CreateLinkedProfileAsync(request.ToCommand(), cancellationToken);
                    return Results.Created(ApiRoutes.AdminProfiles.ForProfile(profile.LegacyUserId), profile.ToResponse());
                }
                catch (AdminProfileAlreadyLinkedException)
                {
                    return Results.Conflict();
                }
            })
            .WithTags("AdminProfiles")
            .WithName("CreateAdminProfile")
            .WithSummary("Создание связанного профиля")
            .WithDescription("Создаёт legacy-профиль пользователя и связывает его с identity-пользователем.")
            .Produces<AdminProfileResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapPut(ApiRoutes.AdminProfiles.Profile, async (
                Guid legacyUserId,
                UpdateAdminProfileRequest request,
                IValidator<UpdateAdminProfileRequest> validator,
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                var profile = await service.UpdateProfileAsync(legacyUserId, request.ToCommand(), cancellationToken);
                return profile is null ? Results.NotFound() : Results.Ok(profile.ToResponse());
            })
            .WithTags("AdminProfiles")
            .WithName("UpdateAdminProfile")
            .WithSummary("Обновление профиля пользователя")
            .WithDescription("Обновляет данные legacy-профиля пользователя.")
            .Produces<AdminProfileResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapPut(ApiRoutes.AdminProfiles.Deactivate, async (
                Guid legacyUserId,
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            {
                var deactivated = await service.DeactivateProfileLinkAsync(legacyUserId, cancellationToken);
                return deactivated ? Results.NoContent() : Results.NotFound();
            })
            .WithTags("AdminProfiles")
            .WithName("DeactivateAdminProfile")
            .WithSummary("Деактивация связи профиля")
            .WithDescription("Отключает связь legacy-профиля с identity-пользователем.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapGet(ApiRoutes.AdminProfiles.CourseAssignableStudents, (
                Guid courseId,
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetAssignableStudentsForCourseAsync(courseId, cancellationToken))
                    .Select(student => student.ToResponse()))))
            .WithTags("AdminProfiles")
            .WithName("GetCourseAssignableStudents")
            .WithSummary("Получение студентов для назначения на курс")
            .WithDescription("Студенты с признаком назначения на выбранный курс. Только для преподавателя-владельца курса.")
            .Produces<IEnumerable<AssignableStudentResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.AdminProfiles.PracticalAssignableStudents, (
                Guid practicalId,
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetAssignableStudentsForPracticalAsync(practicalId, cancellationToken))
                    .Select(student => student.ToResponse()))))
            .WithTags("AdminProfiles")
            .WithName("GetPracticalAssignableStudents")
            .WithSummary("Получение студентов для назначения на практику")
            .WithDescription("Студенты с признаком назначения на выбранную практику. Только для преподавателя-владельца практики.")
            .Produces<IEnumerable<AssignableStudentResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Courses.CourseStudents, async (
                Guid courseId,
                UpdateCourseStudentsRequest request,
                IValidator<UpdateCourseStudentsRequest> validator,
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return await EndpointResults.ExecuteTeacherCommandAsync(() =>
                    service.SetCourseStudentsAsync(request.ToCommand(courseId), cancellationToken));
            })
            .WithTags("AdminProfiles")
            .WithName("SetCourseStudents")
            .WithSummary("Назначение студентов на курс")
            .WithDescription("Заменяет набор студентов курса. Только для преподавателя-владельца; неизвестные / не привязанные id → 400.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Practicals.Students, async (
                Guid practicalId,
                UpdatePracticalStudentsRequest request,
                IValidator<UpdatePracticalStudentsRequest> validator,
                IAdminProfilesService service,
                CancellationToken cancellationToken) =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return await EndpointResults.ExecuteTeacherCommandAsync(() =>
                    service.SetPracticalStudentsAsync(request.ToCommand(practicalId), cancellationToken));
            })
            .WithTags("AdminProfiles")
            .WithName("SetPracticalStudents")
            .WithSummary("Назначение студентов на практику")
            .WithDescription("Заменяет набор студентов практики. Только для преподавателя-владельца; неизвестные / не привязанные id → 400.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}

