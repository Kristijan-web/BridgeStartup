namespace Application.DTO.Post;


public class PostApplyDTO
{
    // Sta mi je sve poterbno od podataka kada korisnik apply-uje na post?
    // - Fajl treba da upload-a korisnik
    // - Treba mi id  korisnika
    // - id post-a na koji apply-uje

    public long UserId { get; set; }
    public long PostId { get; set; }
    public IFormFile userFile { get; set; }

}

