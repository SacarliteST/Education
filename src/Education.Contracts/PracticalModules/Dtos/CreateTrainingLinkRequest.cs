namespace Education.Contracts.PracticalModules;

/// <summary>Запрос ссылки входа в банк заданий.</summary>
/// <param name="ReturnPath">
/// Относительный путь на платформе для кнопки «На платформу» в модуле (например, <c>/student/training</c>);
/// <see langword="null"/> — без кнопки.
/// </param>
public sealed record CreateTrainingLinkRequest(string? ReturnPath);
