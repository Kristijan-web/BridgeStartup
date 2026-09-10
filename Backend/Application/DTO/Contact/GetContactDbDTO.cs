using Application.DTO.User;

namespace Application.DTO.Contact
{
    public class GetAllContactsDbDTO
    {
        public string Subject { get; set; }
        public string Message { get; set; }

        // Treba mi user objekat

        public UserDbDTO User { get; set; }
    }
}
