using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class Answer
{
    [Column("id")]
    public long Id { get; set; }
    [Column("answer", TypeName = "jsonb")]
    public string Answers { get; set; }
    [Column("practical_material_bind_question_id")]
    public long PracticalMaterialBindQuestionId { get; set; }
    public PracticalMaterialBindQuestion PracticalMaterialBindQuestion { get; set; }

    [Column("test_result_id")]
    public long TestResultId { get; set; }
    public TestResult TestResult { get; set; }
}
