namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Учебная группа студентов.
/// </summary>
/// <param name="Name">Название группы.</param>
/// <param name="StudentsCount">Число привязанных студентов в группе.</param>
public sealed record StudentGroupResponse(string Name, int StudentsCount);
