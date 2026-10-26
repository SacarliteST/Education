namespace Education.Application.PracticalModules;

/// <summary>Модуль ответил, что сессия уже не активна (завершена или истекла), и обновить её нельзя.</summary>
/// <param name="message">Описание отказа.</param>
public sealed class ModuleSessionClosedException(string message) : ModulePushFailedException(message);
