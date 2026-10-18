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

        app.MapGet(ApiRoutes.Grades.PracticalStudentGrades, async (
                Guid practicalId,
                IGradesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var rows = await service.GetPracticalStudentGradesAsync(practicalId, cancellationToken);
                return rows is null ? Results.Forbid() : Results.Ok(rows.Select(row => row.ToResponse()));
            }))
            .WithTags("Grades")
            .WithName("GetPracticalStudentGrades")
            .WithSummary("Оценки студентов по практике (преподаватель)")
            .WithDescription(
                "Итоговая оценка по практике (шкала 2–5, то же правило, что видит сам студент) для каждого назначенного " +
                "студента. status: passed — не ниже 3, failed — ниже, incomplete — условия практики не выполнены. " +
                "Доступ: преподаватель — владелец курса практики.")
            .Produces<IEnumerable<PracticalStudentGradeResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Grades.CourseGradebook, async (
                Guid courseId,
                IGradesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetCourseGradebookAsync(courseId, cancellationToken)).ToResponse())))
            .WithTags("Grades")
            .WithName("GetCourseGradebook")
            .WithSummary("Ведомость курса (преподаватель)")
            .WithDescription(
                "Студенты курса × практики курса: оценка 2–5 и статус каждой ячейки, число назначенных и зачтённых практик, " +
                "средняя оценка. Студенты — назначенные на курс или на любую его практику. Доступ: владелец курса.")
            .Produces<CourseGradebookResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}

