namespace Education.Application.TestResults;

public sealed class TestAttemptNotFoundException(long practicalId) : Exception
{
    public long PracticalId { get; } = practicalId;
}
