using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Запрос запуска/продолжения попытки внешнего модуля.</summary>
public sealed record StartModuleSessionRequest(Guid TaskId);
