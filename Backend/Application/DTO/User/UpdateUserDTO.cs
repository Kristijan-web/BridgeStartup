namespace Application.DTO.User
{
    public class UpdateUserDTO
    {
        public long UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
