namespace Education.Application.AdminProfiles;

/// <summary>
/// Параметры постраничного списка студентов для назначения.
/// </summary>
/// <param name="Search">Подстрока в ФИО или логине без учёта регистра; пусто — без поиска.</param>
/// <param name="Assigned"><see langword="true"/> — только назначенные, <see langword="false"/> — только не назначенные, <see langword="null"/> — все.</param>
/// <param name="Page">Номер страницы, начиная с 1.</param>
/// <param name="PageSize">Размер страницы.</param>
public sealed record AssignableStudentsQuery(string? Search, bool? Assigned, int Page, int PageSize)
{
    /// <summary>Максимальный размер страницы.</summary>
    public const int MaxPageSize = 200;
}
