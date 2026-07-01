using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class TheoreticalMaterialFile
{
    [Column("id")]
    public long Id { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [Column("path")]
    public string Path { get; set; }
    [Column("theoretical_material_id")]
    public long TheoreticalMaterialId { get; set; }
    public TheoreticalMaterial TheoreticalMaterial { get; set; }
}
