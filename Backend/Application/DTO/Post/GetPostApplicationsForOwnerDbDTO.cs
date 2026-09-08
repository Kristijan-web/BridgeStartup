public class GetPostApplicationsForOwnerDbDTO
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public UserDTO User { get; set; }

    public IEnumerable<string> Badges { get; set; } = new List<string>();

    public IEnumerable<PostApplicationDTO> Applications { get; set; }
        = new List<PostApplicationDTO>();
}

public class UserDTO
{
    public string Username { get; set; }
    public string Email { get; set; }
}

public class PostApplicationDTO
{
    public long UserId { get; set; }
    public string FilePath { get; set; }
}