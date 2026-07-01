using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class PracticalMaterialBindQuestion
{
    [Column("id")]
    public long Id { get; set; }
    [Column("question_id")]
    public long QuestionId { get; set; }
    public Question Question { get; set; }
    [Column("practical_material_id")]
    public long PracticalMaterialId { get; set; }
    public PracticalMaterial PracticalMaterial { get; set; }

    public List<Answer> Answers { get; set; }
}
