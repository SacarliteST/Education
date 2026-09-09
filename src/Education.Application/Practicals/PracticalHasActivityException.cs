namespace Education.Application.Practicals;

/// <summary>
/// Практику нельзя привязать к внешнему модулю: по ней уже есть работа студентов
/// (загруженные файлы или результаты теста). Нужна новая практика.
/// </summary>
public sealed class PracticalHasActivityException()
    : Exception("По практике уже есть работа студентов — привяжите модуль к новой практике.");
