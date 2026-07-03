using Education.Domain.Common;

namespace Education.Domain.Practicals;

/// <summary>
/// Комментарий к файлу решения практического задания.
/// </summary>
public sealed class CaseFileComment : Entity
{
    /// <summary>
    /// Текст комментария.
    /// </summary>
    public string Text { get; private set; } = String.Empty;

    /// <summary>
    /// Признак автоматически сгенерированного комментария.
    /// </summary>
    public bool IsGenerated { get; private set; }

    /// <summary>
    /// Дата создания комментария.
    /// </summary>
    public DateTime Created { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Идентификатор файла решения.
    /// </summary>
    public Guid CaseFileId { get; private set; }

    /// <summary>
    /// Файл решения, к которому относится комментарий.
    /// </summary>
    public CaseFile CaseFile { get; private set; } = null!;

    private CaseFileComment()
    {
    }

    /// <summary>
    /// Создает комментарий к файлу решения.
    /// </summary>
    /// <param name="caseFileId">Идентификатор файла решения.</param>
    /// <param name="text">Текст комментария.</param>
    /// <param name="isGenerated">Признак автоматически сгенерированного комментария.</param>
    public CaseFileComment(Guid caseFileId, string text, bool isGenerated)
    {
        CaseFileId = caseFileId;
        Text = text;
        IsGenerated = isGenerated;
    }
}

