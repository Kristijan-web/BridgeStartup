namespace Domain
{
    public class PostApplication 
    {

        public string FilePath { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }
        public long PostId { get; set; }
        public virtual Post Post { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}
