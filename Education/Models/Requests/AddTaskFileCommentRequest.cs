using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests
{
    public class AddTaskFileCommentRequest
    {
        [Required]
        public long TaskFileId { get; set; }
        [Required]
        public string Comment { get; set; }
    }
}
