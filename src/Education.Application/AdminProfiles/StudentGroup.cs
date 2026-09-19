namespace Education.Application.AdminProfiles;

/// <summary>
/// Учебная группа студентов.
/// </summary>
/// <param name="Name">Название группы.</param>
/// <param name="StudentsCount">Число привязанных студентов в группе.</param>
public sealed record StudentGroup(string Name, int StudentsCount);
