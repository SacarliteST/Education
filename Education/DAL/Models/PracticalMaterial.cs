using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class PracticalMaterial
{
    [Column("id")]
    public long Id { get; set; }
    [Column("pm_name")]
    public string Name { get; set; }
    [Column("module_id")]
    public long ModuleId { get; set; }
    [Column("is_public")]
    public bool IsPublic { get; set; }
    [Column("tries_count")]
    public int TriesCount { get; set; }
    [Column("percent_for_five")]
    public double PercentForFive { get; set; }
    [Column("percent_for_four")]
    public double PercentForFour { get; set; }
    [Column("percent_for_three")]
    public double PercentForThree { get; set; }

    public Module Module { get; set; }

    public List<Case> Cases { get; set; }
    public List<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; set; }
    public List<TestResult> TestResults { get; set; }
    public List<PracticalBindUser> PracticalBindUsers { get; set; }
}
