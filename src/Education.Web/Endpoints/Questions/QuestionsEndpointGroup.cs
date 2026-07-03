using Education.Application.Questions;
using Education.Contracts;
using Education.Contracts.Questions;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class QuestionsEndpointGroup
{
    public static IEndpointRouteBuilder MapQuestionsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Modules.ModuleQuestions, async (
                Guid moduleId,
                IQuestionsService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetQuestionsAsync(moduleId, cancellationToken)).Select(question => question.ToResponse())))
            .WithTags("Questions")
            .WithName("GetModuleQuestions")
            .WithSummary("Получение вопросов модуля")
            .WithDescription("Возвращает вопросы, созданные преподавателем для выбранного модуля.")
            .Produces<IEnumerable<QuestionResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
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
            .WithName("CreateQuestion")
            .WithSummary("Создание вопроса")
            .WithDescription("Создаёт вопрос для тестирования внутри модуля преподавателя.")
            .Produces<QuestionResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Questions.Question, async (
                Guid questionId,
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
            .WithName("UpdateQuestion")
            .WithSummary("Обновление вопроса")
            .WithDescription("Изменяет текст, тело, ответ, вес и тип вопроса.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Questions.Question, async (
                Guid questionId,
                IQuestionsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteQuestionAsync(questionId, cancellationToken)))
            .WithTags("Questions")
            .WithName("DeleteQuestion")
            .WithSummary("Удаление вопроса")
            .WithDescription("Удаляет вопрос преподавателя из банка вопросов.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}

