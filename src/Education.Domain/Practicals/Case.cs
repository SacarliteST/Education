using Education.Domain.Common;

namespace Education.Domain.Practicals;

/// <summary>
/// Задание практического материала.
/// </summary>
public sealed class Case : Entity
{
    /// <summary>
    /// Название задания.
    /// </summary>
    public string Name { get; private set; } = String.Empty;

    /// <summary>
    /// Текст задания.
    /// </summary>
    public string Text { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор практического материала.
    /// </summary>
    public Guid PracticalMaterialId { get; private set; }

    /// <summary>
    /// Практический материал, к которому относится задание.
    /// </summary>
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;

    /// <summary>
    /// Файлы решений, загруженные по заданию.
    /// </summary>
    public List<CaseFile> CaseFiles { get; private set; } = [];

    private Case()
    {
    }

    /// <summary>
    /// Создает задание практического материала.
    /// </summary>
    /// <param name="practicalMaterialId">Идентификатор практического материала.</param>
    /// <param name="name">Название задания.</param>
    /// <param name="text">Текст задания.</param>
    public Case(Guid practicalMaterialId, string name, string text)
    {
        PracticalMaterialId = practicalMaterialId;
        Name = name;
        Text = text;
    }

    /// <summary>
    /// Обновляет текст задания.
    /// </summary>
    /// <param name="text">Новый текст задания.</param>
    public void UpdateText(string text) => Text = text;
}

