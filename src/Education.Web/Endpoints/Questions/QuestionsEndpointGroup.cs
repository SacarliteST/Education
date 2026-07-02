using Education.Application.Questions;
using Education.Contracts;
using Education.Contracts.Questions;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

public static class QuestionsEndpointGroup
{
    public static IEndpointRouteBuilder MapQuestionsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Modules.ModuleQuestions, async (
                long moduleId,
                IQuestionsService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetQuestionsAsync(moduleId, cancellationToken)).Select(question => question.ToResponse())))
            .WithTags("Questions")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Questions.QuestionsList, async (
                CreateQuestionRequest request,
                IValidator<CreateQuestionRequest> validator,
                IQuestionsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return Results.Ok((await service.CreateQuestionAsync(request.ToCommand(), cancellationToken)).ToResponse());
            }))
            .WithTags("Questions")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Questions.Question, async (
                long questionId,
                UpdateQuestionRequest request,
                IValidator<UpdateQuestionRequest> validator,
                IQuestionsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.UpdateQuestionAsync(questionId, request.ToCommand(), cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("Questions")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Questions.Question, async (
                long questionId,
                IQuestionsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteQuestionAsync(questionId, cancellationToken)))
            .WithTags("Questions")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}
