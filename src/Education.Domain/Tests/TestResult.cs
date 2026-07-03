using Education.Domain.Common;
using Education.Domain.Practicals;
using Education.Domain.Users;

namespace Education.Domain.Tests;

/// <summary>
/// Результат прохождения теста пользователем.
/// </summary>
public sealed class TestResult : Entity
{
    /// <summary>
    /// Дата начала прохождения теста.
    /// </summary>
    public DateTime StatedDate { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата завершения прохождения теста.
    /// </summary>
    public DateTime? TurnedDate { get; private set; }

    /// <summary>
    /// Номер попытки.
    /// </summary>
    public int TryNumber { get; private set; }

    /// <summary>
    /// Признак завершенного теста.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Набранный балл.
    /// </summary>
    public double? Score { get; private set; }

    /// <summary>
    /// Максимальный балл.
    /// </summary>
    public double? MaxScore { get; private set; }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Пользователь, проходивший тест.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Идентификатор практического материала.
    /// </summary>
    public Guid PracticalMaterialId { get; private set; }

    /// <summary>
    /// Практический материал, по которому проходился тест.
    /// </summary>
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;

    /// <summary>
    /// Ответы пользователя в рамках результата теста.
    /// </summary>
    public List<Answer> Answers { get; private set; } = [];

    private TestResult()
    {
    }

    /// <summary>
    /// Создает результат прохождения теста.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="practicalMaterialId">Идентификатор практического материала.</param>
    /// <param name="tryNumber">Номер попытки.</param>
    public TestResult(Guid userId, Guid practicalMaterialId, int tryNumber)
    {
        UserId = userId;
        PracticalMaterialId = practicalMaterialId;
        TryNumber = tryNumber;
    }

    /// <summary>
    /// Завершает прохождение теста и сохраняет набранный результат.
    /// </summary>
    /// <param name="score">Набранный балл.</param>
    /// <param name="maxScore">Максимальный балл.</param>
    /// <param name="turnedDate">Дата завершения теста.</param>
    public void Complete(double score, double maxScore, DateTime turnedDate)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Test result is already completed.");
        }

        Score = score;
        MaxScore = maxScore;
        TurnedDate = turnedDate;
        IsCompleted = true;
    }
}

