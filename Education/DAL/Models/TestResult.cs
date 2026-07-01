using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class TestResult
{
    [Column("id")]
    public long Id { get; set; }
    [Column("started_at")]
    public DateTime StatedDate { get; set; } = DateTime.UtcNow;
    [Column("turned_at")]
    public DateTime? TurnedDate { get; set; }
    [Column("try_number")]
    public int TryNumber { get; set; }
    [Column("is_completed")]
    public bool IsCompleted { get; set; }
    [Column("score")]
    public double? Score { get; set; }
    [Column("max_score")]
    public double? MaxScore { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }
    public User User { get; set; }

    [Column("practical_material_id")]
    public long PracticalMaterialId { get; set; }
    public PracticalMaterial PracticalMaterial { get; set; }
    public List<Answer> Answers { get; set; } = new();
}
