namespace Education.Application.TestResults;

public sealed class StudentPracticalAccessDeniedException(Guid practicalId) : Exception
{
    public Guid PracticalId { get; } = practicalId;
}

