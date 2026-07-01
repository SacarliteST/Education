using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests
{
    public class AddTaskFileRequest
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public long TaskId { get; set; }
    }
}
