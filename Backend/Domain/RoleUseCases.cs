namespace Domain
{
    public class RoleUseCases : Entity
    {



        public long UseCasesId { get; set; }
        public virtual UseCases UseCases { get; set; }
        public long RoleId { get; set; }
        public virtual Role Role { get; set; }

    }
}
