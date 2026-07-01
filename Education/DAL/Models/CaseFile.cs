using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class CaseFile
{
    [Column("id")]
    public long Id { get; set; }
    [Column("path")]
    public string Path { get; set; }
    [Column("case_id")]
    public long CaseId { get; set; }
    public Case Case { get; set; }
    [Column("user_id")]
    public long UserId { get; set; }
    public User User { get; set; }

    [Column("is_accepted")]
    public bool IsAccepted { get; set; }
    [Column("grade")]
    public int Grade { get; set; }

    public List<CaseFileComment> Comments { get; set; }
}
