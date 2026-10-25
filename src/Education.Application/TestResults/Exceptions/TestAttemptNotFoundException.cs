namespace Education.Application.TestResults;

public sealed class TestAttemptNotFoundException(Guid practicalId) : Exception
{
    public Guid PracticalId { get; } = practicalId;
}

