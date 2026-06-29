namespace Domain
{
    public class UseCases : Entity
    {
        // imace samo naziv use case-a

        public string UseCaseId { get; set; }

        public virtual HashSet<RoleUseCases> RoleUsesCase { get; set; }

    }
}
