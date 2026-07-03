using Education.Domain.Common;
using Education.Domain.Materials;
using Education.Domain.Practicals;
using Education.Domain.Tests;

namespace Education.Domain.Courses;

/// <summary>
/// Модуль курса, содержащий теорию, практику и вопросы.
/// </summary>
public sealed class Module : Entity
{
    /// <summary>
    /// Название модуля.
    /// </summary>
    public string Name { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор курса.
    /// </summary>
    public Guid CourseId { get; private set; }

    /// <summary>
    /// Курс, к которому относится модуль.
    /// </summary>
    public Course Course { get; private set; } = null!;

    /// <summary>
    /// Практические материалы модуля.
    /// </summary>
    public List<PracticalMaterial> PracticalMaterials { get; private set; } = [];

    /// <summary>
    /// Вопросы модуля.
    /// </summary>
    public List<Question> Questions { get; private set; } = [];

    /// <summary>
    /// Теоретические материалы модуля.
    /// </summary>
    public List<TheoreticalMaterial> TheoreticalMaterials { get; private set; } = [];

    private Module()
    {
    }

    /// <summary>
    /// Создает модуль курса.
    /// </summary>
    /// <param name="courseId">Идентификатор курса.</param>
    /// <param name="name">Название модуля.</param>
    public Module(Guid courseId, string name)
    {
        CourseId = courseId;
        Name = name;
    }
}

