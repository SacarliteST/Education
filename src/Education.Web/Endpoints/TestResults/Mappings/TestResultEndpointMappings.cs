using Education.Application.TestResults;
using Education.Contracts.TestResults;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «результаты тестов» между контрактами API и слоем приложения.</summary>
internal static class TestResultEndpointMappings
{
    /// <summary>Преобразует <see cref="SubmitTestRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="practicalId">Идентификатор практики.</param>
    public static SubmitTestCommand ToCommand(this SubmitTestRequest request, Guid practicalId)
    {
        return new SubmitTestCommand(
            practicalId,
            request.Answers.Select(answer => new SubmitAnswerCommand(answer.Id, answer.Answer)).ToList());
    }

    /// <summary>Преобразует <see cref="TestStatus"/> в ответ API.</summary>
    /// <param name="status">Исходные данные.</param>
    public static TestStatusResponse ToResponse(this TestStatus status)
    {
        return new TestStatusResponse(status.IsStarted, status.TryNumber);
    }

    /// <summary>Преобразует номер начатой попытки в ответ API.</summary>
    /// <param name="tryNumber">Исходные данные.</param>
    public static StartTestResponse ToStartResponse(this int tryNumber)
    {
        return new StartTestResponse(tryNumber);
    }

    /// <summary>Преобразует <see cref="TestQuestions"/> в ответ API.</summary>
    /// <param name="questions">Исходные данные.</param>
    public static TestQuestionsResponse ToResponse(this TestQuestions questions)
    {
        return new TestQuestionsResponse(
            questions.Questions.Select(question => question.ToResponse()).ToList(),
            questions.IsCompleted);
    }

    /// <summary>Преобразует <see cref="TestQuestion"/> в ответ API.</summary>
    /// <param name="question">Исходные данные.</param>
    public static TestQuestionResponse ToResponse(this TestQuestion question)
    {
        return new TestQuestionResponse(question.Id, question.Text, question.Type, question.Body);
    }

    /// <summary>Преобразует <see cref="TestProtocolSummary"/> в ответ API.</summary>
    /// <param name="protocol">Исходные данные.</param>
    public static TestProtocolSummaryResponse ToResponse(this TestProtocolSummary protocol)
    {
        return new TestProtocolSummaryResponse(
            protocol.Id,
            protocol.UserId,
            protocol.Score,
            protocol.MaxScore,
            protocol.TryNumber,
            protocol.Grade,
            protocol.StudentName,
            protocol.StartedAt,
            protocol.SubmittedAt,
            protocol.DurationSeconds);
    }

    /// <summary>Преобразует <see cref="TestProtocol"/> в ответ API.</summary>
    /// <param name="protocol">Исходные данные.</param>
    public static TestProtocolResponse ToResponse(this TestProtocol protocol)
    {
        return new TestProtocolResponse(
            protocol.Answers.Select(answer => answer.ToResponse()).ToList(),
            protocol.TryNumber,
            protocol.Score,
            protocol.MaxScore,
            protocol.Grade);
    }

    private static TestProtocolAnswerResponse ToResponse(this TestProtocolAnswer answer)
    {
        return new TestProtocolAnswerResponse(
            answer.QuestionId,
            answer.QuestionText,
            answer.QuestionWeight,
            answer.QuestionScore,
            answer.UserAnswer,
            answer.IsCorrect,
            answer.QuestionType,
            answer.QuestionBody);
    }
}

