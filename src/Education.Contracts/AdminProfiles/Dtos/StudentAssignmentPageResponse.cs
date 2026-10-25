namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Страница студентов для назначения на курс или практический материал.
/// </summary>
/// <param name="Items">Студенты запрошенной страницы, отсортированные по фамилии и имени.</param>
/// <param name="TotalCount">Число студентов, подходящих под поиск и фильтр.</param>
/// <param name="AssignedCount">Число всех назначенных студентов — не зависит от поиска и фильтра.</param>
/// <param name="Page">Номер возвращённой страницы, начиная с 1.</param>
/// <param name="PageSize">Размер страницы.</param>
public sealed record StudentAssignmentPageResponse(
    IReadOnlyList<StudentAssignmentEntryResponse> Items,
    int TotalCount,
    int AssignedCount,
    int Page,
    int PageSize);
