using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class QuestionType
{
    [Column("id")]
    public long Id { get; set; }
    [Column("qt_name")]
    public string Name { get; set; }
}
