using Education.Application.Grades;
using Education.Contracts;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

public static class GradesEndpointGroup
{
    public static IEndpointRouteBuilder MapGradesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Grades.PracticalGrade, async (
                long practicalId,
                IGradesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.GetPracticalGradeAsync(practicalId, cancellationToken)).ToResponse())))
            .WithTags("Grades")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        return app;
    }
}
