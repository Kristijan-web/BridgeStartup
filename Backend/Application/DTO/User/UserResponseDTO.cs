namespace Application.DTO.User
{
    public class UserResponseDTO
    {
        // Koje podatke zelim da vratim korisniku?

        // - Email
        // - Username
        // - Naziv role

        public long Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        public string Role { get; set; }
    }
}
