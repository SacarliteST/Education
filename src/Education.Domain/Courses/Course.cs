using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Courses;

/// <summary>
/// Учебный курс, объединяющий модули и материалы.
/// </summary>
public sealed class Course : Entity
{
    /// <summary>
    /// Название курса.
    /// </summary>
    public string Name { get; private set; } = String.Empty;

    /// <summary>
    /// Описание курса.
    /// </summary>
    public string Description { get; private set; } = String.Empty;

    /// <summary>
    /// Дата создания или публикации курса.
    /// </summary>
    public DateTimeOffset Date { get; private set; }

    /// <summary>
    /// Идентификатор автора курса.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Тесты, практика и сдача заданий доступны только во время пары группы (<see cref="ClassWindow"/>); теория — всегда.
    /// </summary>
    public bool PracticeOnlyDuringClass { get; private set; }

    /// <summary>
    /// Автор курса.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Модули курса.
    /// </summary>
    public List<Module> Modules { get; private set; } = [];

    /// <summary>
    /// Связи курса с пользователями, которым он доступен.
    /// </summary>
    public List<CourseBindUser> CourseBindUsers { get; private set; } = [];

    private Course()
    {
    }

    /// <summary>Включает или выключает режим «практика только на парах».</summary>
    /// <param name="value"><see langword="true"/> — тесты и практика доступны только во время пары.</param>
    public void SetPracticeOnlyDuringClass(bool value)
    {
        PracticeOnlyDuringClass = value;
    }

    /// <summary>
    /// Создает учебный курс.
    /// </summary>
    /// <param name="name">Название курса.</param>
    /// <param name="description">Описание курса.</param>
    /// <param name="date">Дата создания или публикации курса.</param>
    /// <param name="userId">Идентификатор автора курса.</param>
    public Course(string name, string description, DateTimeOffset date, Guid userId)
    {
        Name = name;
        Description = description;
        // PostgreSQL `timestamp with time zone` через Npgsql принимает только UTC-DateTimeOffset.
        Date = date.ToUniversalTime();
        UserId = userId;
    }
}

