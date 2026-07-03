using Education.Application.TestResults;
using Education.Contracts;
using Education.Contracts.TestResults;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class TestResultsEndpointGroup
{
    public static IEndpointRouteBuilder MapTestResultsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.TestResults.Status, async (
                Guid practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.GetStatusAsync(practicalId, cancellationToken)).ToResponse())))
            .WithTags("TestResults")
            .WithName("GetTestStatus")
            .WithSummary("Получение статуса теста")
            .WithDescription("Возвращает состояние теста студента по выбранной практике.")
            .Produces<TestStatusResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPut(ApiRoutes.TestResults.Start, async (
                Guid practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.StartTestAsync(practicalId, cancellationToken)).ToStartResponse())))
            .WithTags("TestResults")
            .WithName("StartTest")
            .WithSummary("Запуск теста")
            .WithDescription("Создаёт новую попытку прохождения теста для текущего студента.")
            .Produces<StartTestResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TestResults.Questions, async (
                Guid practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
            {
                var questions = await service.GetQuestionsAsync(practicalId, cancellationToken);
                return questions is null ? Results.NotFound() : Results.Ok(questions.ToResponse());
            }))
            .WithTags("TestResults")
            .WithName("GetTestQuestions")
            .WithSummary("Получение вопросов теста")
            .WithDescription("Возвращает вопросы активной попытки тестирования студента.")
            .Produces<TestQuestionsResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPost(ApiRoutes.TestResults.Submit, async (
                Guid practicalId,
                SubmitTestRequest request,
                IValidator<SubmitTestRequest> validator,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return Results.Ok((await service.SubmitTestAsync(request.ToCommand(practicalId), cancellationToken)).ToResponse());
            }))
            .WithTags("TestResults")
            .WithName("SubmitTest")
            .WithSummary("Отправка ответов теста")
            .WithDescription("Сохраняет ответы студента и завершает попытку тестирования.")
            .Produces<TestProtocolResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TestResults.PracticalProtocols, async (
                Guid practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.GetStudentProtocolsAsync(practicalId, cancellationToken)).Select(protocol => protocol.ToResponse()))))
            .WithTags("TestResults")
            .WithName("GetStudentPracticalProtocols")
            .WithSummary("Получение протоколов студента")
            .WithDescription("Возвращает протоколы прохождения теста текущим студентом по выбранной практике.")
            .Produces<IEnumerable<TestProtocolSummaryResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TestResults.TeacherPracticalProtocols, async (
                Guid practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetTeacherProtocolsAsync(practicalId, cancellationToken)).Select(protocol => protocol.ToResponse()))))
            .WithTags("TestResults")
            .WithName("GetTeacherPracticalProtocols")
            .WithSummary("Получение протоколов практики")
            .WithDescription("Возвращает протоколы студентов по практике преподавателя.")
            .Produces<IEnumerable<TestProtocolSummaryResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.TestResults.Protocol, async (
                Guid testResultId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            {
                var protocol = await service.GetProtocolAsync(testResultId, cancellationToken);
                return protocol is null ? Results.NotFound() : Results.Ok(protocol.ToResponse());
            })
            .WithTags("TestResults")
            .WithName("GetTestProtocol")
            .WithSummary("Получение протокола теста")
            .WithDescription("Возвращает детальный протокол прохождения теста.")
            .Produces<TestProtocolResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}

