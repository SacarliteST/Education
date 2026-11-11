namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Сценарии правил случайной выборки заданий и наборов студентов.</summary>
public interface ITaskSelectionRulesService
{
    /// <summary>Правило практики для преподавателя-владельца курса или <see langword="null"/>.</summary>
    Task<TaskSelectionRuleInfo?> GetRuleAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать или заменить правило практики. Практика должна быть внешней и без ручного списка заданий.
    /// </summary>
    /// <exception cref="PracticalNotExternalException">Практика не внешняя.</exception>
    /// <exception cref="TaskSelectionModeConflictException">У практики есть ручной список заданий.</exception>
    Task<UpsertTaskSelectionRuleResult> UpsertRuleAsync(
        Guid practicalId, UpsertTaskSelectionRuleCommand command, CancellationToken cancellationToken = default);

    /// <summary>Удалить правило (вернуть практику в фиксированный режим). <see langword="false"/> — правила нет.</summary>
    /// <exception cref="TaskSelectionRuleLockedException">По назначениям уже есть сессии модуля.</exception>
    Task<bool> DeleteRuleAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>Таблица «студент → задания» по практике на пару для преподавателя.</summary>
    Task<IReadOnlyList<StudentAssignmentRow>> ListAssignmentsAsync(
        Guid practicalId, Guid classWindowId, CancellationToken cancellationToken = default);

    /// <summary>Пересобрать набор студента на пару (только до первой сессии по нему).</summary>
    /// <exception cref="AssignmentRegenerationForbiddenException">По набору уже есть сессии модуля.</exception>
    Task RegenerateAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Набор текущего студента по практике. Если пара идёт, а набора ещё нет — формирует его лениво.
    /// <see langword="null"/> — правила нет (фиксированный режим) или набор ещё не сформирован.
    /// </summary>
    Task<MyAssignmentsInfo?> GetMyAssignmentsAsync(Guid practicalId, CancellationToken cancellationToken = default);
}
