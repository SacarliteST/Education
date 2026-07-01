using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class Question
{
    [Column("id")]
    public long Id { get; set; }
    [Column("question_text")]
    public string Text { get; set; }
    [Column("question_body", TypeName = "jsonb")]
    public string Options { get; set; }
    [Column("answer", TypeName = "jsonb")]
    public string Answer { get; set; }
    [Column("weight")]
    public double Weight { get; set; }
    [Column("question_type_id")]
    public long QuestionTypeId { get; set; }
    public QuestionType QuestionType { get; set; }
    [Column("module_id")]
    public long ModuleId { get; set; }
    public Module Module { get; set; }

    public List<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; set; }
}
