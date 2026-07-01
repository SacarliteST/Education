using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class TheoreticalMaterial
{
    [Column("id")]
    public long Id { get; set; }
    [Column("tm_name")]
    public string Name { get; set; }
    [Column("lecture_text")]
    public string Text { get; set; }
    [Column("module_id")]
    public long ModuleId { get; set; }
    public Module Module { get; set; }

    public List<TheoreticalMaterialFile> Files { get; set; }
    public List<TheoreticalMaterialLink> Links { get; set; }
}
