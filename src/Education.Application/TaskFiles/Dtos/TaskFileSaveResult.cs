using Education.Domain.Practicals;

namespace Education.Application.TaskFiles;

/// <summary>
/// Результат сохранения файла сдачи вместе с ключом заменённого файла.
/// </summary>
/// <param name="TaskFile">Файл сдачи после сохранения метаданных.</param>
/// <param name="ReplacedStorageKey">Ключ файла, который был заменён новой сдачей.</param>
public sealed record TaskFileSaveResult(CaseFile TaskFile, string? ReplacedStorageKey);

