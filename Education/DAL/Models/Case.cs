using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class Case
{
    [Column("id")]
    public long Id { get; set; }
    [Column("pm_name")]
    public string Name { get; set; }
    [Column("case_text")]
    public string Text { get; set; }
    [Column("practical_material_id")]
    public long PracticalMaterialId { get; set; }
    public PracticalMaterial PracticalMaterial { get; set; }

    public List<CaseFile> CaseFiles { get; set; }
}
