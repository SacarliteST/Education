namespace Education.Application.TestResults;

/// <summary>Студент не назначен на практику, доступ запрещён.</summary>
/// <param name="practicalId">Идентификатор практики.</param>
public sealed class StudentPracticalAccessDeniedException(Guid practicalId) : Exception
{
    /// <summary>Идентификатор практики.</summary>
    public Guid PracticalId { get; } = practicalId;
}

