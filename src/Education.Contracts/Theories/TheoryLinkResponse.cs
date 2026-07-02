namespace Education.Contracts.Theories;

/// <summary>
/// Данные ссылки теоретического материала, возвращаемые API системы обучения.
/// </summary>
public sealed record TheoryLinkResponse(
    long Id,
    string Link,
    string Description);
