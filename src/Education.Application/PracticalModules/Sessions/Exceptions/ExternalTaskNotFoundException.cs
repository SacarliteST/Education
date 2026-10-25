namespace Education.Application.PracticalModules;

/// <summary>Практика не является внешней, либо задание не привязано к модулю.</summary>
public sealed class ExternalTaskNotFoundException() : Exception("Внешнее задание не найдено.");
