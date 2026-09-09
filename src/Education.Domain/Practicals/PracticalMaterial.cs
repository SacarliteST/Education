using Education.Domain.Common;
using Education.Domain.Courses;
using Education.Domain.Tests;

namespace Education.Domain.Practicals;

/// <summary>
/// Практический материал модуля.
/// </summary>
public sealed class PracticalMaterial : Entity
{
    /// <summary>
    /// Название практического материала.
    /// </summary>
    public string Name { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор модуля.
    /// </summary>
    public Guid ModuleId { get; private set; }

    /// <summary>
    /// Признак опубликованного материала.
    /// </summary>
    public bool IsPublic { get; private set; }

    /// <summary>
    /// Вид практики: внутренняя (тест/задания на платформе) или внешняя (модуль).
    /// </summary>
    public PracticalKind Kind { get; private set; } = PracticalKind.Internal;

    /// <summary>
    /// Лимит времени на одну попытку внешнего модуля в минутах. <see langword="null"/> — без лимита.
    /// </summary>
    public int? TimeLimitMinutes { get; private set; }

    /// <summary>
    /// Количество доступных попыток прохождения теста.
    /// </summary>
    public int TriesCount { get; private set; } = 1;

    /// <summary>
    /// Минимальный процент результата для оценки 5.
    /// </summary>
    public double PercentForFive { get; private set; } = 90;

    /// <summary>
    /// Минимальный процент результата для оценки 4.
    /// </summary>
    public double PercentForFour { get; private set; } = 75;

    /// <summary>
    /// Минимальный процент результата для оценки 3.
    /// </summary>
    public double PercentForThree { get; private set; } = 60;

    /// <summary>
    /// Модуль, к которому относится практический материал.
    /// </summary>
    public Module Module { get; private set; } = null!;

    /// <summary>
    /// Задания практического материала.
    /// </summary>
    public List<Case> Cases { get; private set; } = [];

    /// <summary>
    /// Связи практического материала с вопросами.
    /// </summary>
    public List<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; private set; } = [];

    /// <summary>
    /// Результаты прохождения теста по практическому материалу.
    /// </summary>
    public List<TestResult> TestResults { get; private set; } = [];

    /// <summary>
    /// Связи практического материала с пользователями.
    /// </summary>
    public List<PracticalBindUser> PracticalBindUsers { get; private set; } = [];

    private PracticalMaterial()
    {
    }

    /// <summary>
    /// Создает практический материал.
    /// </summary>
    /// <param name="moduleId">Идентификатор модуля.</param>
    /// <param name="name">Название практического материала.</param>
    public PracticalMaterial(Guid moduleId, string name)
    {
        ModuleId = moduleId;
        Name = name;
    }

    /// <summary>
    /// Публикует практический материал.
    /// </summary>
    public void Publish()
    {
        IsPublic = true;
    }

    /// <summary>
    /// Переводит практику на внешний модуль: вид <c>external</c>, лимит попыток и времени.
    /// </summary>
    /// <param name="triesCount">Количество запусков (та же семантика, что у внутреннего теста).</param>
    /// <param name="timeLimitMinutes">Лимит времени на попытку в минутах или <see langword="null"/>.</param>
    public void BindExternalModule(int triesCount, int? timeLimitMinutes)
    {
        if (triesCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(triesCount));
        }

        if (timeLimitMinutes is < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timeLimitMinutes));
        }

        Kind = PracticalKind.External;
        TriesCount = triesCount;
        TimeLimitMinutes = timeLimitMinutes;
    }

    /// <summary>
    /// Проверяет, может ли пользователь начать новую попытку.
    /// </summary>
    /// <param name="completedAttemptCount">Количество уже завершенных попыток.</param>
    /// <returns><see langword="true"/>, если доступна новая попытка.</returns>
    public bool CanStartAttempt(int completedAttemptCount) => completedAttemptCount < TriesCount;

    /// <summary>
    /// Настраивает параметры прохождения теста.
    /// </summary>
    /// <param name="triesCount">Количество доступных попыток.</param>
    /// <param name="percentForFive">Минимальный процент результата для оценки 5.</param>
    /// <param name="percentForFour">Минимальный процент результата для оценки 4.</param>
    /// <param name="percentForThree">Минимальный процент результата для оценки 3.</param>
    public void ConfigureTest(int triesCount, double percentForFive, double percentForFour, double percentForThree)
    {
        if (triesCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(triesCount));
        }

        TriesCount = triesCount;
        PercentForFive = percentForFive;
        PercentForFour = percentForFour;
        PercentForThree = percentForThree;
    }

    /// <summary>
    /// Рассчитывает оценку за тест по набранному результату.
    /// </summary>
    /// <param name="score">Набранный балл.</param>
    /// <param name="maxScore">Максимальный балл.</param>
    /// <returns>Оценка за тест.</returns>
    public int CalculateTestGrade(double? score, double? maxScore)
    {
        return GradingPolicy.GetGrade(score, maxScore, PercentForFive, PercentForFour, PercentForThree);
    }

    /// <summary>
    /// Рассчитывает итоговую оценку по тесту и заданиям.
    /// </summary>
    /// <param name="bestTestGrade">Лучшая оценка за тест.</param>
    /// <param name="meanTaskGrade">Средняя оценка за задания.</param>
    /// <returns>Итоговая оценка.</returns>
    public int CalculateFinalGrade(int bestTestGrade, double meanTaskGrade)
    {
        return (int)Math.Ceiling((bestTestGrade + meanTaskGrade) / 2);
    }
}

