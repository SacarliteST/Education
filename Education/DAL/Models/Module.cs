using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class Module
{
    [Column("id")]
    public long Id { get; set; }
    [Column("m_name")]
    public string Name { get; set; }
    [Column("course_id")]
    public long CourseId { get; set; }
    public Course Course { get; set; }

    public List<PracticalMaterial> PracticalMaterials { get; set; }
    public List<Question> Questions { get; set; }
    public List<TheoreticalMaterial> TheoreticalMaterials { get; set; }
}
