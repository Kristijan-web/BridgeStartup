namespace Application.DTO.Post
{
    public class PostsResponseDTO
    {

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public UserDTO User { get; set; }

    }

    public class UserDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
