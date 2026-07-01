using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests
{
    public class AddLinkRequest
    {
        [Required]
        public string Description { get; set; }
        [Required, Url]
        public string Link { get; set; }
        [Required]
        public long TheoryMaterialId { get; set; }
    }
}
