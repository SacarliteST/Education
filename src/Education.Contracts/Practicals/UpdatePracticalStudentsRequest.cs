namespace Education.Contracts.Practicals;

/// <summary>
/// Запрос на обновление набора студентов, назначенных на практический материал.
/// </summary>
/// <param name="UserIds">Полный желаемый набор идентификаторов студентов учебной системы.</param>
public sealed record UpdatePracticalStudentsRequest(
    IReadOnlyList<Guid> UserIds);
