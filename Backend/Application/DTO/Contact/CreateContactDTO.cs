namespace Application.DTO.Contact
{
    public class CreateContactDTO
    {
        public long UserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

    }
}
