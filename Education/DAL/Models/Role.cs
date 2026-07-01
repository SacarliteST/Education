using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class Role
{
    [Column("id")]
    public long Id { get; set; }
    [Column("r_name")]
    public string Name { get; set; }
}
