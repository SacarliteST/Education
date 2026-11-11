namespace Education.Contracts.Practicals;

/// <summary>Задание из случайного набора студента с состоянием последней сессии по нему.</summary>
/// <param name="AssignmentId">Идентификатор назначения.</param>
/// <param name="ExternalTaskRef">Ключ задания внутри модуля.</param>
/// <param name="SortOrder">Порядок задания в наборе.</param>
/// <param name="LastState">Состояние последней сессии (active/completed/expired) или <see langword="null"/> — сессий не было.</param>
/// <param name="BestGrade">Оценка последней завершённой сессии или <see langword="null"/>.</param>
public sealed record AssignmentTaskResponse(
    Guid AssignmentId,
    string ExternalTaskRef,
    int SortOrder,
    string? LastState,
    int? BestGrade);
