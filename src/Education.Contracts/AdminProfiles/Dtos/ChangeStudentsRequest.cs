namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Запрос на изменение набора студентов курса или практики точечно: добавить и убрать.
/// Остальные назначения не затрагиваются, поэтому одновременные правки разных преподавателей не перезаписывают друг друга.
/// </summary>
/// <param name="Add">Студенты, которых нужно назначить. Уже назначенные пропускаются.</param>
/// <param name="Remove">Студенты, с которых нужно снять назначение. Не назначенные пропускаются.</param>
public sealed record ChangeStudentsRequest(
    IReadOnlyList<Guid> Add,
    IReadOnlyList<Guid> Remove);
