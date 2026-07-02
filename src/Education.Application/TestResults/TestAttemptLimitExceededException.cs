namespace Education.Application.TestResults;

public sealed class TestAttemptLimitExceededException(long practicalId) : Exception
{
    public long PracticalId { get; } = practicalId;
}
