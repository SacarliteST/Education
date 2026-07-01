using System.Text.Json;
using System.Text.Json.Serialization;
using Education.Consts;
using Education.DAL.Models;
using Education.Models.QuestionAnswers;
using Education.Models.Questions;

namespace Education.Helpers;

public static class ScoreHelper
{
    public static int GetGrade(double? score, double? maxScore, double five, double four, double three)
    {
        if (score is null || maxScore is null)
        {
            return 0;
        }

        var percent = score / maxScore;
        if (percent >= five / 100)
        {
            return 5;
        }

        if (percent >= four / 100)
        {
            return 4;
        }

        if (percent >= three / 100)
        {
            return 3;
        }

        return 2;
    }

    public static double GetScore(QuestionTypeEnum type, string answer, string userAnswer)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        double result = 0;
        switch (type)
        {
            case QuestionTypeEnum.SingleChoice:
                var origSingleAnswer = JsonSerializer.Deserialize<SingleChoiceAnswerModel>(answer, options);
                if (userAnswer == origSingleAnswer!.CorrectAnswerId)
                {
                    result = 1;
                }
                return result;
            case QuestionTypeEnum.MultipleChoice:
                var origMultipleAnswer = JsonSerializer.Deserialize<MultipleAnswerModel>(answer, options);
                var userMultipleAnswer = JsonSerializer.Deserialize<List<string>>(userAnswer, options);
                foreach (var multOption in origMultipleAnswer!.Answers)
                {
                    var isIn = userMultipleAnswer!.Contains(multOption.Id);
                    if (isIn && multOption.Correct)
                    {
                        result += multOption.Weight;
                    }

                    if (!isIn && !multOption.Correct)
                    {
                        result += multOption.Weight;
                    }
                }
                return result;
            case QuestionTypeEnum.Match:
                var origMatchAnswer = JsonSerializer.Deserialize<MatchAnswerModel>(answer, options);
                var userMatchAnswer = JsonSerializer.Deserialize<List<MatchUserAnswerModel>>(userAnswer, options);
                foreach (var matchOption in origMatchAnswer!.Matches)
                {
                    var userMatch = userMatchAnswer!.FirstOrDefault(m => m.Left == matchOption.Left.Id);
                    if (userMatch is null)
                    {
                        continue;
                    }

                    if (matchOption.Right.Id == userMatch.Right)
                    {
                        result += matchOption.Weight;
                    }
                }
                return result;
            case QuestionTypeEnum.ShortAnswer:
                var origShortAnswer = JsonSerializer.Deserialize<ShortAnswerModel>(answer, options);
                if (origShortAnswer!.Answer.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Any(a => a.Equals(userAnswer, StringComparison.InvariantCultureIgnoreCase)))
                {
                    result = 1;
                }
                return result;
            default:
                return 0;
        }
    }

    public static AnswerBase GetAnswer(QuestionTypeEnum type, Question answer, string userAnswer)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        switch (type)
        {
            case QuestionTypeEnum.SingleChoice:
                var origSingleAnswer = JsonSerializer.Deserialize<SingleChoiceAnswerModel>(answer.Answer, options);
                var singleResult = new SingleAnswerBase
                {
                    QuestionText = answer.Text,
                    QuestionWeight = answer.Weight,
                    QuestionScore = answer.Weight * GetScore(type, answer.Answer, userAnswer),
                    Type = type,
                };
                foreach (var ans in origSingleAnswer!.Answers)
                {
                    var isAnswer = ans.Id == userAnswer;
                    singleResult.Answers.Add(new SingleOption
                    {
                        Id = ans.Id,
                        Text = ans.Text,
                        IsAnswer = isAnswer,
                        IsCorrect = isAnswer && ans.Id == origSingleAnswer.CorrectAnswerId
                    });
                }
                return singleResult;
            case QuestionTypeEnum.MultipleChoice:
                var origMultipleAnswer = JsonSerializer.Deserialize<MultipleAnswerModel>(answer.Answer, options);
                var userMultipleAnswer = JsonSerializer.Deserialize<List<string>>(userAnswer, options);
                var multipleAnswer = new MultipleAnswerBase
                {
                    QuestionText = answer.Text,
                    QuestionWeight = answer.Weight,
                    QuestionScore = answer.Weight * GetScore(type, answer.Answer, userAnswer),
                    Type = type,
                };
                foreach (var ans in origMultipleAnswer!.Answers)
                {
                    var isAnswer = userMultipleAnswer!.Contains(ans.Id);
                    var isCorrect = (isAnswer && ans.Correct) || (!isAnswer && !ans.Correct);
                    multipleAnswer.Options.Add(new Option
                    {
                        Id = ans.Id,
                        Text = ans.Text,
                        IsAnswer = isAnswer,
                        IsCorrect = isCorrect,
                        Weight = isCorrect ? answer.Weight * ans.Weight : 0
                    });
                }
                return multipleAnswer;
            case QuestionTypeEnum.Match:
                var origMatchAnswer = JsonSerializer.Deserialize<MatchAnswerModel>(answer.Answer, options);
                var userMatchAnswer = JsonSerializer.Deserialize<List<MatchUserAnswerModel>>(userAnswer, options);
                var matchResult = new MatchAnswerBase
                {
                    QuestionText = answer.Text,
                    QuestionWeight = answer.Weight,
                    QuestionScore = answer.Weight * GetScore(type, answer.Answer, userAnswer),
                    Type = type,
                };
                foreach (var ans in userMatchAnswer!)
                {
                    var match = origMatchAnswer!.Matches.FirstOrDefault(m => m.Left.Id == ans.Left);
                    if (match is null)
                    {
                        continue;
                    }

                    var isCorrect = ans.Right == match.Right.Id;
                    matchResult.Matches.Add(new Match
                    {
                        Left = match.Left.Text,
                        Right = match.Right.Text,
                        IsCorrect = isCorrect,
                        Weight = isCorrect ? answer.Weight * match.Weight : 0
                    });
                }
                return matchResult;
            case QuestionTypeEnum.ShortAnswer:
                var origShortAnswer = JsonSerializer.Deserialize<ShortAnswerModel>(answer.Answer, options);
                var shortResult = new ShortAnswerBase
                {
                    QuestionText = answer.Text,
                    QuestionWeight = answer.Weight,
                    QuestionScore = answer.Weight * GetScore(type, answer.Answer, userAnswer),
                    Type = type,
                };
                shortResult.IsCorrect =
                    origShortAnswer!.Answer.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Any(a => a.Equals(userAnswer, StringComparison.InvariantCultureIgnoreCase));
                shortResult.Answer = userAnswer;
                return shortResult;
            default:
                return null;
        }
    }
}
