using Education.Application.Questions;
using Education.Contracts.Questions;
using Education.Domain.Tests;

namespace Education.Web.Endpoints;

internal static class QuestionEndpointMappings
{
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

    public static UpdateQuestionCommand ToCommand(this UpdateQuestionRequest request)
    {
        return new UpdateQuestionCommand(
            request.Text,
            request.Body,
            request.Answer,
            request.Weight,
            request.Type);
    }

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
