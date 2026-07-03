using Education.Domain.Common;

namespace Education.Domain.Materials;

/// <summary>
/// Ссылка, прикрепленная к теоретическому материалу.
/// </summary>
public sealed class TheoreticalMaterialLink : Entity
{
    /// <summary>
    /// Описание ссылки.
    /// </summary>
    public string Description { get; private set; } = String.Empty;

    /// <summary>
    /// Адрес ссылки.
    /// </summary>
    public string Link { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор теоретического материала.
    /// </summary>
    public Guid TheoreticalMaterialId { get; private set; }

    /// <summary>
    /// Теоретический материал, к которому прикреплена ссылка.
    /// </summary>
    public TheoreticalMaterial TheoreticalMaterial { get; private set; } = null!;

    private TheoreticalMaterialLink()
    {
    }

    /// <summary>
    /// Создает ссылку теоретического материала.
    /// </summary>
    /// <param name="theoreticalMaterialId">Идентификатор теоретического материала.</param>
    /// <param name="description">Описание ссылки.</param>
    /// <param name="link">Адрес ссылки.</param>
    public TheoreticalMaterialLink(Guid theoreticalMaterialId, string description, string link)
    {
        TheoreticalMaterialId = theoreticalMaterialId;
        Description = description;
        Link = link;
    }
}

