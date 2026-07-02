namespace Education.Application.TestResults;

public sealed class StudentPracticalAccessDeniedException(long practicalId) : Exception
{
    public long PracticalId { get; } = practicalId;
}
