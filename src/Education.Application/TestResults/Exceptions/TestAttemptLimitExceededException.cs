namespace Education.Application.TestResults;

/// <summary>Лимит попыток прохождения теста исчерпан.</summary>
/// <param name="practicalId">Идентификатор практики.</param>
public sealed class TestAttemptLimitExceededException(Guid practicalId) : Exception
{
    /// <summary>Идентификатор практики.</summary>
    public Guid PracticalId { get; } = practicalId;
}

