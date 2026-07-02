namespace Education.Application.Theories;

/// <summary>
/// Команда добавления ссылки к теоретическому материалу.
/// </summary>
/// <param name="TheoryMaterialId">Идентификатор теоретического материала.</param>
/// <param name="Link">Адрес ссылки.</param>
/// <param name="Description">Описание ссылки.</param>
public sealed record CreateTheoryLinkCommand(long TheoryMaterialId, string Link, string Description);
