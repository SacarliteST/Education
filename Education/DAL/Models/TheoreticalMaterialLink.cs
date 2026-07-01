using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class TheoreticalMaterialLink
{
    [Column("id")]
    public long Id { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [Column("link")]
    public string Link { get; set; }
    [Column("theoretical_material_id")]
    public long TheoreticalMaterialId { get; set; }
    public TheoreticalMaterial TheoreticalMaterial { get; set; }
}
