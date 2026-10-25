using Education.Application.Questions;
using Education.Contracts.Questions;
using Education.Domain.Tests;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «вопросы» между контрактами API и слоем приложения.</summary>
internal static class QuestionEndpointMappings
{
    /// <summary>Преобразует <see cref="CreateQuestionRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static CreateQuestionCommand ToCommand(this CreateQuestionRequest request)
    {
        return new CreateQuestionCommand(
            request.ModuleId,
            request.Type,
            request.Text,
            request.Body,
            request.Answer,
            request.Weight);
    }

    /// <summary>Преобразует <see cref="UpdateQuestionRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static UpdateQuestionCommand ToCommand(this UpdateQuestionRequest request)
    {
        return new UpdateQuestionCommand(
            request.Text,
            request.Body,
            request.Answer,
            request.Weight,
            request.Type);
    }

    /// <summary>Преобразует <see cref="Question"/> в ответ API.</summary>
    /// <param name="question">Исходные данные.</param>
    public static QuestionResponse ToResponse(this Question question)
    {
        return new QuestionResponse(
            question.Id,
            question.Text,
            question.QuestionTypeId,
            question.Weight,
            question.Answer);
    }
}

