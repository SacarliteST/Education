using Education.Domain.Common;
using Education.Domain.Practicals;
using Education.Domain.Users;

namespace Education.Domain.Tests;

public sealed class TestResult : Entity
{
    public DateTime StatedDate { get; private set; } = DateTime.UtcNow;
    public DateTime? TurnedDate { get; private set; }
    public int TryNumber { get; private set; }
    public bool IsCompleted { get; private set; }
    public double? Score { get; private set; }
    public double? MaxScore { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public long PracticalMaterialId { get; private set; }
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;
    public List<Answer> Answers { get; private set; } = [];

    private TestResult()
    {
    }

    public TestResult(long userId, long practicalMaterialId, int tryNumber)
    {
        UserId = userId;
        PracticalMaterialId = practicalMaterialId;
        TryNumber = tryNumber;
    }

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
