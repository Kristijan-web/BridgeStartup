namespace Application.DTO.Post
{
    public class CreatePostDTO
    {
        // Koja polja su potrebna za kreiranja POST-a?
        // - Title
        // - Description
        // - Email
        // - Phone
        // - Badgevi
        // - UserId

        public long UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IEnumerable<long> Badges { get; set; }

    }
}
