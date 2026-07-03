namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Студент, доступный для назначения на курс или практический материал.
/// </summary>
/// <param name="LegacyUserId">Идентификатор пользователя в учебной системе.</param>
/// <param name="FullName">Полное имя пользователя.</param>
/// <param name="IsAssigned">Признак текущего назначения.</param>
public sealed record AssignableStudentResponse(Guid LegacyUserId, string FullName, bool IsAssigned);


