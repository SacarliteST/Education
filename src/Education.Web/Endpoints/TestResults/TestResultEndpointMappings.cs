using Education.Application.TestResults;
using Education.Contracts.TestResults;
using Education.Domain.Tests;

namespace Education.Web.Endpoints;

internal static class TestResultEndpointMappings
{
    public static SubmitTestCommand ToCommand(this SubmitTestRequest request, long practicalId)
    {
        return new SubmitTestCommand(
            practicalId,
            request.Answers.Select(answer => new SubmitAnswerCommand(answer.Id, answer.Answer)).ToList());
    }

    public static TestStatusResponse ToResponse(this TestStatus status)
    {
        return new TestStatusResponse(status.IsStarted, status.TryNumber);
    }

    public static StartTestResponse ToStartResponse(this int tryNumber)
    {
        return new StartTestResponse(tryNumber);
    }

    public static TestQuestionsResponse ToResponse(this TestQuestions questions)
    {
        return new TestQuestionsResponse(
            questions.Questions.Select(question => question.ToResponse()).ToList(),
            questions.IsCompleted);
    }

    public static TestQuestionResponse ToResponse(this TestQuestion question)
    {
        return new TestQuestionResponse(question.Id, question.Text, question.Type, question.Body);
    }

    public static TestProtocolSummaryResponse ToResponse(this TestProtocolSummary protocol)
    {
        return new TestProtocolSummaryResponse(
            protocol.Id,
            protocol.UserId,
            protocol.Score,
            protocol.MaxScore,
            protocol.TryNumber,
            protocol.Grade);
    }

    public static TestProtocolResponse ToResponse(this TestProtocol protocol)
    {
        return new TestProtocolResponse(
            protocol.Answers.Select(answer => answer.ToResponse()).ToList(),
            protocol.TryNumber,
            protocol.Score,
            protocol.MaxScore,
            protocol.Grade);
    }

    private static TestProtocolAnswerResponse ToResponse(this QuestionAnswerScore answer)
    {
        return new TestProtocolAnswerResponse(
            answer.QuestionId,
            answer.QuestionText,
            answer.QuestionWeight,
            answer.QuestionScore,
            answer.UserAnswer,
            answer.IsCorrect);
    }
}
