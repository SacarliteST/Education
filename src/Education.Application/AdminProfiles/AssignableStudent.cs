namespace Education.Application.AdminProfiles;

/// <summary>
/// Студент, которого можно назначить на курс или практический материал.
/// </summary>
/// <param name="LegacyUserId">Идентификатор пользователя в учебной системе.</param>
/// <param name="FullName">Полное имя пользователя.</param>
/// <param name="IsAssigned">Признак текущего назначения.</param>
public sealed record AssignableStudent(Guid LegacyUserId, string FullName, bool IsAssigned);

