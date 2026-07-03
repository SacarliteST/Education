using System.Text.Json;
using System.Text.Json.Serialization;

namespace Education.Domain.Tests;

public static class QuestionScoringService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public static QuestionAnswerScore Score(Question question, string userAnswer)
    {
        var scoreFactor = GetScoreFactor(QuestionTypeIds.ToKind(question.QuestionTypeId), question.Answer, userAnswer);
        var questionScore = question.Weight * scoreFactor;

        return new QuestionAnswerScore(
            question.Id,
            question.Text,
            question.Weight,
            questionScore,
            userAnswer,
            Math.Abs(questionScore - question.Weight) < Double.Epsilon);
    }

    public static double GetScoreFactor(QuestionKind type, string answer, string userAnswer)
    {
        return type switch
        {
            QuestionKind.SingleChoice => GetSingleChoiceScore(answer, userAnswer),
            QuestionKind.MultipleChoice => GetMultipleChoiceScore(answer, userAnswer),
            QuestionKind.Match => GetMatchScore(answer, userAnswer),
            QuestionKind.ShortAnswer => GetShortAnswerScore(answer, userAnswer),
            _ => 0,
        };
    }

    private static double GetSingleChoiceScore(string answer, string userAnswer)
    {
        var correctAnswer = JsonSerializer.Deserialize<SingleChoiceAnswerPayload>(answer, JsonOptions);
        return correctAnswer?.CorrectAnswerId == userAnswer ? 1 : 0;
    }

    private static double GetMultipleChoiceScore(string answer, string userAnswer)
    {
        var correctAnswer = JsonSerializer.Deserialize<MultipleChoiceAnswerPayload>(answer, JsonOptions);
        var selectedIds = JsonSerializer.Deserialize<List<string>>(userAnswer, JsonOptions) ?? [];

        return correctAnswer?.Answers.Sum(option =>
        {
            var isSelected = selectedIds.Contains(option.Id);
            return (isSelected && option.Correct) || (!isSelected && !option.Correct)
                ? option.Weight
                : 0;
        }) ?? 0;
    }

    private static double GetMatchScore(string answer, string userAnswer)
    {
        var correctAnswer = JsonSerializer.Deserialize<MatchAnswerPayload>(answer, JsonOptions);
        var selectedMatches = JsonSerializer.Deserialize<List<MatchUserAnswerPayload>>(userAnswer, JsonOptions) ?? [];

        return correctAnswer?.Matches.Sum(match =>
        {
            var selectedMatch = selectedMatches.FirstOrDefault(item => item.Left == match.Left.Id);
            return selectedMatch?.Right == match.Right.Id ? match.Weight : 0;
        }) ?? 0;
    }

    private static double GetShortAnswerScore(string answer, string userAnswer)
    {
        var correctAnswer = JsonSerializer.Deserialize<ShortAnswerPayload>(answer, JsonOptions);
        return correctAnswer?.Answer
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(item => item.Equals(userAnswer, StringComparison.InvariantCultureIgnoreCase)) == true
            ? 1
            : 0;
    }

    private sealed record SingleChoiceAnswerPayload(IReadOnlyList<SingleChoiceOptionPayload> Answers, string CorrectAnswerId);

    private sealed record SingleChoiceOptionPayload(string Id, string Text);

    private sealed record MultipleChoiceAnswerPayload(IReadOnlyList<MultipleChoiceOptionPayload> Answers);

    private sealed record MultipleChoiceOptionPayload(string Id, string Text, bool Correct, double Weight);

    private sealed record MatchAnswerPayload(IReadOnlyList<MatchOptionPayload> Matches);

    private sealed record MatchOptionPayload(MatchHalfPayload Left, MatchHalfPayload Right, double Weight);

    private sealed record MatchHalfPayload(string Id, string Text);

    private sealed record MatchUserAnswerPayload(string Right, string Left);

    private sealed record ShortAnswerPayload(string Answer);
}

