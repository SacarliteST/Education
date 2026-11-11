using Education.Domain.Common;

namespace Education.Domain.Practicals;

/// <summary>
/// Правило случайной выборки заданий внешнего модуля для практики:
/// при открытии пары платформа формирует каждому студенту персональный
/// набор заданий из каталога модуля по этому правилу.
/// Практика с правилом не может иметь ручных <see cref="Case"/> и наоборот.
/// </summary>
public sealed class TaskSelectionRule : Entity
{
    /// <summary>Минимальное количество заданий в наборе студента.</summary>
    public const int MinTaskCount = 1;

    /// <summary>Максимальное количество заданий в наборе студента.</summary>
    public const int MaxTaskCount = 10;

    /// <summary>
    /// Практический материал, к которому относится правило (1:1).
    /// </summary>
    public Guid PracticalMaterialId { get; private set; }

    /// <summary>
    /// Внешний модуль, из каталога которого выбираются задания.
    /// </summary>
    public Guid PracticalModuleId { get; private set; }

    /// <summary>
    /// Сколько заданий получает каждый студент за пару.
    /// </summary>
    public int TaskCount { get; private set; }

    /// <summary>
    /// Допустимые уровни сложности заданий модуля (шкала модуля).
    /// Пустой список — любая сложность.
    /// </summary>
    public List<short> DifficultyLevels { get; private set; } = [];

    /// <summary>
    /// Допустимые темы модуля (идентификаторы тем из каталога модуля).
    /// Пустой список — любые темы.
    /// </summary>
    public List<string> TopicRefs { get; private set; } = [];

    /// <summary>
    /// Практический материал, к которому относится правило.
    /// </summary>
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;

    private TaskSelectionRule()
    {
    }

    /// <summary>
    /// Создаёт правило случайной выборки для практики.
    /// </summary>
    /// <param name="practicalMaterialId">Идентификатор практического материала.</param>
    /// <param name="practicalModuleId">Идентификатор внешнего модуля, из каталога которого выбираются задания.</param>
    /// <param name="taskCount">Количество заданий в наборе студента.</param>
    /// <param name="difficultyLevels">Допустимые уровни сложности; пусто — любые.</param>
    /// <param name="topicRefs">Допустимые темы модуля; пусто — любые.</param>
    public TaskSelectionRule(
        Guid practicalMaterialId,
        Guid practicalModuleId,
        int taskCount,
        IEnumerable<short>? difficultyLevels = null,
        IEnumerable<string>? topicRefs = null)
    {
        PracticalMaterialId = practicalMaterialId;
        PracticalModuleId = practicalModuleId;
        Update(taskCount, difficultyLevels, topicRefs);
    }

    /// <summary>
    /// Заменяет параметры правила.
    /// </summary>
    /// <param name="taskCount">Количество заданий в наборе студента.</param>
    /// <param name="difficultyLevels">Допустимые уровни сложности; пусто — любые.</param>
    /// <param name="topicRefs">Допустимые темы модуля; пусто — любые.</param>
    public void Update(
        int taskCount,
        IEnumerable<short>? difficultyLevels = null,
        IEnumerable<string>? topicRefs = null)
    {
        if (taskCount is < MinTaskCount or > MaxTaskCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(taskCount), taskCount,
                $"Количество заданий должно быть от {MinTaskCount} до {MaxTaskCount}.");
        }

        TaskCount = taskCount;
        DifficultyLevels = (difficultyLevels ?? []).Distinct().OrderBy(level => level).ToList();
        TopicRefs = (topicRefs ?? []).Where(topicRef => !String.IsNullOrWhiteSpace(topicRef))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }
}
