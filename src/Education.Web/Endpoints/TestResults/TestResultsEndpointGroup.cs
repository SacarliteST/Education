using Education.Application.TestResults;
using Education.Contracts;
using Education.Contracts.TestResults;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

public static class TestResultsEndpointGroup
{
    public static IEndpointRouteBuilder MapTestResultsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.TestResults.Status, async (
                long practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.GetStatusAsync(practicalId, cancellationToken)).ToResponse())))
            .WithTags("TestResults")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPut(ApiRoutes.TestResults.Start, async (
                long practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.StartTestAsync(practicalId, cancellationToken)).ToStartResponse())))
            .WithTags("TestResults")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TestResults.Questions, async (
                long practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
            {
                var questions = await service.GetQuestionsAsync(practicalId, cancellationToken);
                return questions is null ? Results.NotFound() : Results.Ok(questions.ToResponse());
            }))
            .WithTags("TestResults")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPost(ApiRoutes.TestResults.Submit, async (
                long practicalId,
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
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TestResults.PracticalProtocols, async (
                long practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
                Results.Ok((await service.GetStudentProtocolsAsync(practicalId, cancellationToken)).Select(protocol => protocol.ToResponse()))))
            .WithTags("TestResults")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TestResults.TeacherPracticalProtocols, async (
                long practicalId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetTeacherProtocolsAsync(practicalId, cancellationToken)).Select(protocol => protocol.ToResponse()))))
            .WithTags("TestResults")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.TestResults.Protocol, async (
                long testResultId,
                ITestResultsService service,
                CancellationToken cancellationToken) =>
            {
                var protocol = await service.GetProtocolAsync(testResultId, cancellationToken);
                return protocol is null ? Results.NotFound() : Results.Ok(protocol.ToResponse());
            })
            .WithTags("TestResults")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}
