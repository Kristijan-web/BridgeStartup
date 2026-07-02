namespace Domain
{
    public class RoleUseCases
    {



        public long UseCasesId { get; set; }
        public virtual UseCases UseCases { get; set; }
        public long RoleId { get; set; }
        public virtual Role Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}
