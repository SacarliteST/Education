namespace Education.Application.AdminProfiles;

/// <summary>
/// Команда точечного изменения набора студентов практического материала.
/// </summary>
/// <param name="PracticalId">Идентификатор практического материала.</param>
/// <param name="Add">Студенты, которых нужно назначить.</param>
/// <param name="Remove">Студенты, с которых нужно снять назначение.</param>
public sealed record ChangePracticalStudentsCommand(Guid PracticalId, IReadOnlyList<Guid> Add, IReadOnlyList<Guid> Remove);
