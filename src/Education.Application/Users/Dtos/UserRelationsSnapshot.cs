namespace Education.Application.Users;

/// <summary>Связи учебного пользователя, которые нужно перенести при слиянии с пользователем Identity.</summary>
/// <param name="LegacyUserId">Прежний учебный идентификатор пользователя.</param>
/// <param name="OwnedCourseIds">Курсы, которыми владеет пользователь.</param>
/// <param name="AssignedCourseIds">Курсы, назначенные пользователю.</param>
/// <param name="AssignedPracticalIds">Практики, назначенные пользователю.</param>
/// <param name="CaseFileIds">Файлы сдачи пользователя.</param>
/// <param name="TestResultIds">Результаты тестов пользователя.</param>
public sealed record UserRelationsSnapshot(
    Guid LegacyUserId,
    IReadOnlySet<Guid> OwnedCourseIds,
    IReadOnlySet<Guid> AssignedCourseIds,
    IReadOnlySet<Guid> AssignedPracticalIds,
    IReadOnlySet<Guid> CaseFileIds,
    IReadOnlySet<Guid> TestResultIds);

