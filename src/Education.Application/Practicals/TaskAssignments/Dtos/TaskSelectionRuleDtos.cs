using Education.Domain.PracticalModules;

namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Команда создания/замены правила случайной выборки заданий практики.</summary>
/// <param name="PracticalModuleId">Внешний модуль, из каталога которого выбираются задания.</param>
/// <param name="TaskCount">Количество заданий в наборе студента.</param>
/// <param name="DifficultyLevels">Допустимые уровни сложности; пусто — любые.</param>
/// <param name="TopicRefs">Допустимые темы модуля; пусто — любые.</param>
public sealed record UpsertTaskSelectionRuleCommand(
    Guid PracticalModuleId,
    int TaskCount,
    IReadOnlyList<short> DifficultyLevels,
    IReadOnlyList<string> TopicRefs);

/// <summary>Правило случайной выборки заданий практики.</summary>
/// <param name="PracticalId">Практика.</param>
/// <param name="PracticalModuleId">Внешний модуль, из каталога которого выбираются задания.</param>
/// <param name="TaskCount">Количество заданий в наборе студента.</param>
/// <param name="DifficultyLevels">Допустимые уровни сложности; пусто — любые.</param>
/// <param name="TopicRefs">Допустимые темы модуля; пусто — любые.</param>
public sealed record TaskSelectionRuleInfo(
    Guid PracticalId,
    Guid PracticalModuleId,
    int TaskCount,
    IReadOnlyList<short> DifficultyLevels,
    IReadOnlyList<string> TopicRefs);

/// <summary>Результат сохранения правила выборки.</summary>
/// <param name="Rule">Сохранённое правило.</param>
/// <param name="EmptySelectionWarning">
/// <see langword="true"/>, если по сохранённому правилу каталог модуля прямо сейчас не даёт
/// ни одного задания (или каталог недоступен) — правило сохранено, но наборы не сформируются.
/// </param>
public sealed record UpsertTaskSelectionRuleResult(TaskSelectionRuleInfo Rule, bool EmptySelectionWarning);

/// <summary>Задание из набора студента с состоянием последней сессии по нему.</summary>
/// <param name="AssignmentId">Идентификатор назначения.</param>
/// <param name="ExternalTaskRef">Ключ задания внутри модуля.</param>
/// <param name="SortOrder">Порядок задания в наборе.</param>
/// <param name="LastState">Состояние последней сессии или <see langword="null"/> — сессий не было.</param>
/// <param name="BestGrade">Лучшая оценка по сессиям или <see langword="null"/>.</param>
public sealed record AssignmentTaskItem(
    Guid AssignmentId,
    string ExternalTaskRef,
    int SortOrder,
    ModuleSessionState? LastState,
    int? BestGrade);

/// <summary>Строка таблицы «студент → задания» для преподавателя.</summary>
/// <param name="StudentId">Идентификатор студента.</param>
/// <param name="StudentName">Отображаемое имя студента.</param>
/// <param name="Tasks">Набор заданий студента на пару.</param>
public sealed record StudentAssignmentRow(
    Guid StudentId,
    string StudentName,
    IReadOnlyList<AssignmentTaskItem> Tasks);

/// <summary>Набор заданий текущего студента на пару.</summary>
/// <param name="ClassWindowId">Пара, к которой относится набор.</param>
/// <param name="Tasks">Набор заданий студента.</param>
public sealed record MyAssignmentsInfo(Guid ClassWindowId, IReadOnlyList<AssignmentTaskItem> Tasks);
