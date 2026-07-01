using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests
{
    public class AddDocRequest
    {
        [Required]
        public string Descritpion { get; set; }
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public long TheoryMaterialId { get; set; }
    }
}
