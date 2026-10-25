namespace Education.Application.TestResults;

/// <summary>У студента нет активной попытки прохождения теста.</summary>
/// <param name="practicalId">Идентификатор практики.</param>
public sealed class TestAttemptNotFoundException(Guid practicalId) : Exception
{
    /// <summary>Идентификатор практики.</summary>
    public Guid PracticalId { get; } = practicalId;
}

