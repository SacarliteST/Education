namespace Education.Application.TestResults;

public sealed class TestAttemptLimitExceededException(Guid practicalId) : Exception
{
    public Guid PracticalId { get; } = practicalId;
}

