using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Contact
{
    public class CreateContactDTO
    {
        [Range(typeof(long), "1", "9223372036854775807")]
        public long UserId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }

    }
}
