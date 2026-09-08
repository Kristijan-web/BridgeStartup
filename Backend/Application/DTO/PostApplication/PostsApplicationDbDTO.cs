namespace Application.DTO.PostApplication
{
    public class PostsApplicationDbDTO
    {


        public string FilePath { get; set; }

        public long UserId { get; set; }

        public long PostId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }



    }
}
