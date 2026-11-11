using Education.Domain.Common;
using Education.Domain.Courses;

namespace Education.Domain.Practicals;

/// <summary>
/// Материализованное назначение задания модуля студенту на конкретную пару:
/// одна строка — одно задание из персонального случайного набора,
/// сформированного платформой по <see cref="TaskSelectionRule"/> при открытии пары.
/// </summary>
public sealed class PracticalTaskAssignment : Entity
{
    /// <summary>
    /// Практический материал, для которого сформирован набор.
    /// </summary>
    public Guid PracticalMaterialId { get; private set; }

    /// <summary>
    /// Студент — владелец набора.
    /// </summary>
    public Guid StudentId { get; private set; }

    /// <summary>
    /// Пара, в окно которой сформирован набор.
    /// </summary>
    public Guid ClassWindowId { get; private set; }

    /// <summary>
    /// Непрозрачный ключ задания внутри внешнего модуля (из каталога модуля).
    /// </summary>
    public string ExternalTaskRef { get; private set; } = String.Empty;

    /// <summary>
    /// Порядок задания в наборе студента.
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Когда назначение сформировано (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Практический материал, для которого сформирован набор.
    /// </summary>
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;

    /// <summary>
    /// Пара, в окно которой сформирован набор.
    /// </summary>
    public ClassWindow ClassWindow { get; private set; } = null!;

    private PracticalTaskAssignment()
    {
    }

    /// <summary>
    /// Создаёт назначение задания студенту на пару.
    /// </summary>
    /// <param name="practicalMaterialId">Идентификатор практического материала.</param>
    /// <param name="studentId">Идентификатор студента.</param>
    /// <param name="classWindowId">Идентификатор пары.</param>
    /// <param name="externalTaskRef">Ключ задания внутри модуля (из каталога).</param>
    /// <param name="sortOrder">Порядок задания в наборе студента.</param>
    /// <param name="createdAt">Момент формирования назначения (UTC).</param>
    public PracticalTaskAssignment(
        Guid practicalMaterialId,
        Guid studentId,
        Guid classWindowId,
        string externalTaskRef,
        int sortOrder,
        DateTimeOffset createdAt)
    {
        if (String.IsNullOrWhiteSpace(externalTaskRef))
        {
            throw new ArgumentException("Ключ задания модуля обязателен.", nameof(externalTaskRef));
        }

        PracticalMaterialId = practicalMaterialId;
        StudentId = studentId;
        ClassWindowId = classWindowId;
        ExternalTaskRef = externalTaskRef;
        SortOrder = sortOrder;
        CreatedAt = createdAt;
    }
}
