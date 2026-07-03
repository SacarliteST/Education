namespace Education.Domain.Tests;

public static class QuestionTypeIds
{
    public static readonly Guid SingleChoice = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid MultipleChoice = Guid.Parse("20000000-0000-0000-0000-000000000002");
    public static readonly Guid Match = Guid.Parse("20000000-0000-0000-0000-000000000003");
    public static readonly Guid ShortAnswer = Guid.Parse("20000000-0000-0000-0000-000000000004");

    public static QuestionKind ToKind(Guid id)
    {
        if (id == SingleChoice)
        {
            return QuestionKind.SingleChoice;
        }

        if (id == MultipleChoice)
        {
            return QuestionKind.MultipleChoice;
        }

        if (id == Match)
        {
            return QuestionKind.Match;
        }

        if (id == ShortAnswer)
        {
            return QuestionKind.ShortAnswer;
        }

        throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown question type identifier.");
    }
}

