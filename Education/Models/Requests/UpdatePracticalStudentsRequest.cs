using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class UpdatePracticalStudentsRequest
{
    [Required]
    public long PracticalId { get; set; }
    [Required]
    public List<long> UserIds { get; set; }
}
