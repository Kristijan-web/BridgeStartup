namespace Domain
{
    public class Contact : Entity
    {
        // KOje kolone su mi potrebne za kontact?
        // - UserId
        // - Subject
        // - Message

        public string Subject { get; set; }
        public string Message { get; set; }



        public long UserId { get; set; }
        public User User { get; set; }

        // Koga referencira Contact? -> navigation prop
        // - User-a

        // Ko referencira Contact?
        // - Niko
    }
}
