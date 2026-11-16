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
    /// Требуемая сложность каждого места в наборе. Например, <c>[1, 1, 3]</c>
    /// означает два задания сложности 1 и одно задание сложности 3.
    /// Пустой список — точное распределение не задано.
    /// </summary>
    public List<short> DifficultyDistribution { get; private set; } = [];

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
    /// <param name="difficultyDistribution">Сложность каждого места в наборе; пусто — без точных квот.</param>
    public TaskSelectionRule(
        Guid practicalMaterialId,
        Guid practicalModuleId,
        int taskCount,
        IEnumerable<short>? difficultyLevels = null,
        IEnumerable<string>? topicRefs = null,
        IEnumerable<short>? difficultyDistribution = null)
    {
        PracticalMaterialId = practicalMaterialId;
        PracticalModuleId = practicalModuleId;
        Update(taskCount, difficultyLevels, topicRefs, difficultyDistribution);
    }

    /// <summary>
    /// Заменяет параметры правила.
    /// </summary>
    /// <param name="taskCount">Количество заданий в наборе студента.</param>
    /// <param name="difficultyLevels">Допустимые уровни сложности; пусто — любые.</param>
    /// <param name="topicRefs">Допустимые темы модуля; пусто — любые.</param>
    /// <param name="difficultyDistribution">Сложность каждого места в наборе; пусто — без точных квот.</param>
    public void Update(
        int taskCount,
        IEnumerable<short>? difficultyLevels = null,
        IEnumerable<string>? topicRefs = null,
        IEnumerable<short>? difficultyDistribution = null)
    {
        if (taskCount is < MinTaskCount or > MaxTaskCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(taskCount), taskCount,
                $"Количество заданий должно быть от {MinTaskCount} до {MaxTaskCount}.");
        }

        var normalizedDifficultyLevels = (difficultyLevels ?? []).Distinct().OrderBy(level => level).ToList();
        var normalizedDistribution = (difficultyDistribution ?? []).OrderBy(level => level).ToList();
        if (normalizedDifficultyLevels.Concat(normalizedDistribution).Any(level => level is < 1 or > 5))
        {
            throw new ArgumentOutOfRangeException(
                nameof(difficultyLevels),
                "Уровень сложности должен быть от 1 до 5.");
        }

        if (normalizedDifficultyLevels.Count > 0 && normalizedDistribution.Count > 0)
        {
            throw new ArgumentException(
                "Нельзя одновременно задавать допустимые сложности и точное распределение по сложности.");
        }

        if (normalizedDistribution.Count > 0 && normalizedDistribution.Count != taskCount)
        {
            throw new ArgumentException(
                "Количество мест в распределении по сложности должно совпадать с количеством заданий.",
                nameof(difficultyDistribution));
        }

        TaskCount = taskCount;
        DifficultyLevels = normalizedDifficultyLevels;
        DifficultyDistribution = normalizedDistribution;
        TopicRefs = (topicRefs ?? []).Where(topicRef => !String.IsNullOrWhiteSpace(topicRef))
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }
}
