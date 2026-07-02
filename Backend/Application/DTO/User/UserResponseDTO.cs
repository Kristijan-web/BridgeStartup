namespace Application.DTO
{
    public class UserResponseDTO
    {
        // Koje podatke zelim da vratim korisniku?

        // - Email
        // - Username
        // - Naziv role

        public string Email { get; set; }
        public string Username { get; set; }

        public string Role { get; set; }
    }
}
