namespace Application.DTO
{
    public class LoginResponseDTO
    {
        // koje podatke vracam korisniku?
        // - Email
        // - Username

        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
