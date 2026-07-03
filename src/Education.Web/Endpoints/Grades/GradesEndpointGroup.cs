using Education.Application.Grades;
using Education.Contracts;
using Education.Contracts.Grades;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

internal static class GradesEndpointGroup
{
    public static IEndpointRouteBuilder MapGradesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Grades.PracticalGrade, async (
                Guid practicalId,
                IGradesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.GetPracticalGradeAsync(practicalId, cancellationToken)).ToResponse())))
            .WithTags("Grades")
            .WithName("GetPracticalGrade")
            .WithSummary("Получение оценки за практику")
            .WithDescription("Возвращает итоговую оценку и поясняющие сообщения по практическому заданию студента.")
            .Produces<PracticalGradeResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        return app;
    }
}

