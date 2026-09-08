using System.ComponentModel.DataAnnotations;
namespace Application.DTO.Post;


public class PostApplyDTO
{
    // Sta mi je sve poterbno od podataka kada korisnik apply-uje na post?
    // - Fajl treba da upload-a korisnik
    // - Treba mi id  korisnika
    // - id post-a na koji apply-uje

    public long UserId { get; set; }
    [Range(1, long.MaxValue)]
    public long PostId { get; set; }
    [Required]
    public IFormFile userFile { get; set; } = null!;

}
