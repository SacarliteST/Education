using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AuthRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Login { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
}
