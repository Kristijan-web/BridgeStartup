namespace Domain
{
    public class PostApplication : Entity
    {

        public string FilePath { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }
        public long PostId { get; set; }
        public virtual Post Post { get; set; }

    }
}
