namespace Education.Application.AdminProfiles;

/// <summary>
/// Команда обновления набора студентов, назначенных на практический материал.
/// </summary>
/// <param name="PracticalId">Идентификатор практического материала.</param>
/// <param name="UserIds">Полный желаемый набор идентификаторов студентов учебной системы.</param>
public sealed record SetPracticalStudentsCommand(Guid PracticalId, IReadOnlyList<Guid> UserIds);
