namespace Education.Contracts.Theories;

/// <summary>
/// Запрос на добавление внешней ссылки к теоретическому материалу.
/// </summary>
public sealed record CreateTheoryLinkRequest(
    Guid TheoryMaterialId,
    string Link,
    string Description);


