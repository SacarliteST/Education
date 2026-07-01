using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AddUserRequest
{
    [Required(AllowEmptyStrings = false)]
    public string FirstName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string LastName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string MiddleName { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Login { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
    [Required]
    public long RoleId { get; set; }
}
