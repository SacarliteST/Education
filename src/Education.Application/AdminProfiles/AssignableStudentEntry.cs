namespace Education.Application.AdminProfiles;

/// <summary>
/// Студент постраничного списка назначения.
/// </summary>
/// <param name="LegacyUserId">Идентификатор пользователя в учебной системе.</param>
/// <param name="FullName">Полное имя пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
/// <param name="IsAssigned">Признак текущего назначения.</param>
public sealed record AssignableStudentEntry(Guid LegacyUserId, string FullName, string Login, bool IsAssigned);
