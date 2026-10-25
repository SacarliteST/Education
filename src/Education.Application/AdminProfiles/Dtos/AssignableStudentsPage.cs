namespace Education.Application.AdminProfiles;

/// <summary>
/// Страница студентов для назначения.
/// </summary>
/// <param name="Items">Студенты страницы.</param>
/// <param name="TotalCount">Число студентов, подходящих под поиск и фильтр.</param>
/// <param name="AssignedCount">Число всех назначенных студентов независимо от поиска и фильтра.</param>
public sealed record AssignableStudentsPage(IReadOnlyList<AssignableStudentEntry> Items, int TotalCount, int AssignedCount);
