namespace Application.DTO.Auth
{
    public class RegisterUserDTO
    {
        // ovo je za podatke
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public string? Link { get; set; }
    }
}
