namespace Application.DTO.User
{
    public class UserDbDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public IEnumerable<string> AllowedUseCases { get; set; } = new List<string>();

    }
}
