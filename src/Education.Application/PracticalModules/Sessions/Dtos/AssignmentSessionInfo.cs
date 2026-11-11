using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>Состояние последней сессии по назначению случайного набора.</summary>
/// <param name="AssignmentId">Идентификатор назначения.</param>
/// <param name="Status">Состояние последней сессии.</param>
/// <param name="Grade">Оценка последней завершённой сессии или <see langword="null"/>.</param>
public sealed record AssignmentSessionInfo(Guid AssignmentId, ModuleSessionState Status, int? Grade);
