using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class CaseFileComment
{
    [Column("id")]
    public long Id { get; set; }
    [Column("cfc_text")]
    public string Text { get; set; }
    [Column("is_generated")]
    public bool IsGenerated { get; set; }
    [Column("created")]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public long CaseFileId { get; set; }
    public CaseFile CaseFile { get; set; }
}
