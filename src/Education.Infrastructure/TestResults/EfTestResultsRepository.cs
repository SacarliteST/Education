using System.Text.Json;
using Education.Application.TestResults;
using Education.Domain.Practicals;
using Education.Domain.Tests;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.TestResults;

internal sealed class EfTestResultsRepository(EducationDbContext context) : ITestResultsRepository
{
    public Task<bool> IsPracticalAssignedToStudentAsync(
        Guid practicalId,
        Guid studentUserId,
        CancellationToken cancellationToken = default)
    {
        return context.PracticalBindUsers.AnyAsync(
            bind => bind.PracticalMaterialId == practicalId && bind.UserId == studentUserId,
            cancellationToken);
    }

    public async Task<TestStatus> GetStatusAsync(
        Guid practicalId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var testResult = await context.TestResults
            .AsNoTracking()
            .FirstOrDefaultAsync(
                result => result.PracticalMaterialId == practicalId && result.UserId == userId && !result.IsCompleted,
                cancellationToken);

        return testResult is null
            ? new TestStatus(false, null)
            : new TestStatus(true, testResult.TryNumber);
    }

    public async Task<int> StartTestAsync(
        Guid practicalId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var activeAttempt = await context.TestResults.FirstOrDefaultAsync(
            result => result.PracticalMaterialId == practicalId && result.UserId == userId && !result.IsCompleted,
            cancellationToken);
        if (activeAttempt is not null)
        {
            return activeAttempt.TryNumber;
        }

        var practical = await context.PracticalMaterials.FirstAsync(
            item => item.Id == practicalId,
            cancellationToken);
        var attemptsCount = await context.TestResults.CountAsync(
            result => result.PracticalMaterialId == practicalId && result.UserId == userId,
            cancellationToken);
        if (!practical.CanStartAttempt(attemptsCount))
        {
            throw new TestAttemptLimitExceededException(practicalId);
        }

        var testResult = new TestResult(userId, practicalId, attemptsCount + 1);
        await context.TestResults.AddAsync(testResult, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return testResult.TryNumber;
    }

    public async Task<TestQuestions?> GetQuestionsAsync(
        Guid practicalId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var activeAttempt = await context.TestResults
            .AsNoTracking()
            .FirstOrDefaultAsync(
                result => result.PracticalMaterialId == practicalId && result.UserId == userId && !result.IsCompleted,
                cancellationToken);
        if (activeAttempt is null)
        {
            return null;
        }

        var questions = await context.Questions
            .AsNoTracking()
            .Where(question => question.PracticalMaterialBindQuestions.Any(bind => bind.PracticalMaterialId == practicalId))
            .OrderByDescending(question => question.Id)
            .Select(question => new TestQuestion(
                question.Id,
                question.Text,
                question.QuestionTypeId,
                question.Options))
            .ToListAsync(cancellationToken);

        return new TestQuestions(questions, activeAttempt.IsCompleted);
    }

    public async Task<TestProtocolSummary> SubmitTestAsync(
        SubmitTestCommand command,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var testResult = await context.TestResults
            .Include(result => result.PracticalMaterial)
            .FirstOrDefaultAsync(
                result => result.PracticalMaterialId == command.PracticalId
                    && result.UserId == userId
                    && !result.IsCompleted,
                cancellationToken);
        if (testResult is null)
        {
            throw new TestAttemptNotFoundException(command.PracticalId);
        }

        var questionIds = command.Answers.Select(answer => answer.QuestionId).ToHashSet();
        var questions = await context.Questions
            .Where(question => questionIds.Contains(question.Id))
            .ToDictionaryAsync(question => question.Id, cancellationToken);
        var binds = await context.PracticalMaterialBindQuestions
            .Where(bind => bind.PracticalMaterialId == command.PracticalId && questionIds.Contains(bind.QuestionId))
            .ToDictionaryAsync(bind => bind.QuestionId, cancellationToken);

        double score = 0;
        double maxScore = 0;
        var answers = new List<Answer>();

        foreach (var submittedAnswer in command.Answers)
        {
            if (!questions.TryGetValue(submittedAnswer.QuestionId, out var question)
                || !binds.TryGetValue(submittedAnswer.QuestionId, out var bind))
            {
                continue;
            }

            var answerScore = QuestionScoringService.Score(question, submittedAnswer.Answer);
            score += answerScore.QuestionScore;
            maxScore += question.Weight;
            answers.Add(new Answer(
                bind.Id,
                testResult.Id,
                JsonSerializer.Serialize(answerScore)));
        }

        testResult.Complete(score, maxScore, DateTime.UtcNow);
        await context.Answers.AddRangeAsync(answers, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return ToSummary(testResult);
    }

    public async Task<IReadOnlyList<TestProtocolSummary>> GetStudentProtocolsAsync(
        Guid practicalId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var results = await context.TestResults
            .Include(result => result.PracticalMaterial)
            .AsNoTracking()
            .Where(result => result.PracticalMaterialId == practicalId && result.UserId == userId && result.IsCompleted)
            .ToListAsync(cancellationToken);

        return results.Select(ToSummary).ToList();
    }

    public async Task<IReadOnlyList<TestProtocolSummary>> GetTeacherProtocolsAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var results = await context.TestResults
            .Include(result => result.PracticalMaterial)
            .AsNoTracking()
            .Where(result => result.PracticalMaterialId == practicalId && result.IsCompleted)
            .ToListAsync(cancellationToken);

        return results.Select(ToSummary).ToList();
    }

    public async Task<TestProtocol?> GetProtocolAsync(Guid testResultId, CancellationToken cancellationToken = default)
    {
        var testResult = await context.TestResults
            .Include(result => result.PracticalMaterial)
            .AsNoTracking()
            .FirstOrDefaultAsync(result => result.Id == testResultId, cancellationToken);
        if (testResult is null or { IsCompleted: false })
        {
            return null;
        }

        var answers = await context.Answers
            .AsNoTracking()
            .Where(answer => answer.TestResultId == testResultId)
            .Select(answer => answer.Answers)
            .ToListAsync(cancellationToken);
        var scores = answers
            .Select(answer => JsonSerializer.Deserialize<QuestionAnswerScore>(answer))
            .Where(answer => answer is not null)
            .Select(answer => answer!)
            .ToList();

        // TD-010: обогащаем ответ типом и телом вопроса, чтобы UI показал текст
        // выбранного варианта, а не его id. Джойн по актуальному вопросу —
        // работает и для старых протоколов (в сериализованном ответе этих полей нет).
        var questionIds = scores.Select(score => score.QuestionId).ToHashSet();
        var questions = await context.Questions
            .AsNoTracking()
            .Where(question => questionIds.Contains(question.Id))
            .Select(question => new { question.Id, question.QuestionTypeId, question.Options })
            .ToDictionaryAsync(question => question.Id, cancellationToken);

        var protocolAnswers = scores
            .Select(score =>
            {
                questions.TryGetValue(score.QuestionId, out var question);
                return new TestProtocolAnswer(
                    score.QuestionId,
                    score.QuestionText,
                    score.QuestionWeight,
                    score.QuestionScore,
                    score.UserAnswer,
                    score.IsCorrect,
                    question?.QuestionTypeId ?? Guid.Empty,
                    question?.Options ?? string.Empty);
            })
            .ToList();

        return new TestProtocol(
            protocolAnswers,
            testResult.TryNumber,
            testResult.Score,
            testResult.MaxScore,
            testResult.PracticalMaterial.CalculateTestGrade(testResult.Score, testResult.MaxScore));
    }

    private static TestProtocolSummary ToSummary(TestResult testResult)
    {
        return new TestProtocolSummary(
            testResult.Id,
            testResult.UserId,
            testResult.Score,
            testResult.MaxScore,
            testResult.TryNumber,
            testResult.PracticalMaterial.CalculateTestGrade(testResult.Score, testResult.MaxScore));
    }
}


