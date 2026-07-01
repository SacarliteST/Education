using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class PracticalBindUser
{
    [Column("id")]
    public long Id { get; set; }
    [Column("practical_material_id")]
    public long PracticalMaterialId { get; set; }
    public PracticalMaterial PracticalMaterial { get; set; }
    [Column("user_id")]
    public long UserId { get; set; }
    public User User { get; set; }
}
