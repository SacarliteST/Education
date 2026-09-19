namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Студент в постраничном списке назначения на курс или практический материал.
/// </summary>
/// <param name="LegacyUserId">Идентификатор пользователя в учебной системе.</param>
/// <param name="FullName">Полное имя пользователя.</param>
/// <param name="Login">Логин пользователя — отличает однофамильцев.</param>
/// <param name="IsAssigned">Признак текущего назначения.</param>
/// <param name="Group">Учебная группа или <see langword="null"/>, если не задана.</param>
public sealed record StudentAssignmentEntryResponse(
    Guid LegacyUserId,
    string FullName,
    string Login,
    bool IsAssigned,
    string? Group = null);
