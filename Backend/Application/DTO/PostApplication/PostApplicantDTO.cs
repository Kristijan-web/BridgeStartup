namespace Application.DTO.PostApplication;

public class PostApplicantDTO
{
    public long UserId { get; set; }
    public string Username { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string FileName { get; set; } = "";
}

public class ApplicationFileDTO
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}

