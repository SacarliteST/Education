using Education.Domain.Common;
using Education.Domain.Courses;

namespace Education.Domain.Materials;

/// <summary>
/// Теоретический материал модуля.
/// </summary>
public sealed class TheoreticalMaterial : Entity
{
    /// <summary>
    /// Название теоретического материала.
    /// </summary>
    public string Name { get; private set; } = String.Empty;

    /// <summary>
    /// Текст теоретического материала.
    /// </summary>
    public string Text { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор модуля.
    /// </summary>
    public Guid ModuleId { get; private set; }

    /// <summary>
    /// Модуль, к которому относится теоретический материал.
    /// </summary>
    public Module Module { get; private set; } = null!;

    /// <summary>
    /// Файлы теоретического материала.
    /// </summary>
    public List<TheoreticalMaterialFile> Files { get; private set; } = [];

    /// <summary>
    /// Ссылки теоретического материала.
    /// </summary>
    public List<TheoreticalMaterialLink> Links { get; private set; } = [];

    private TheoreticalMaterial()
    {
    }

    /// <summary>
    /// Создает теоретический материал.
    /// </summary>
    /// <param name="moduleId">Идентификатор модуля.</param>
    /// <param name="name">Название теоретического материала.</param>
    /// <param name="text">Текст теоретического материала.</param>
    public TheoreticalMaterial(Guid moduleId, string name, string text)
    {
        ModuleId = moduleId;
        Name = name;
        Text = text;
    }

    /// <summary>
    /// Переименовывает теоретический материал.
    /// </summary>
    /// <param name="name">Новое название материала.</param>
    public void Rename(string name) => Name = name;

    /// <summary>
    /// Обновляет текст теоретического материала.
    /// </summary>
    /// <param name="text">Новый текст материала.</param>
    public void UpdateText(string text) => Text = text;
}

