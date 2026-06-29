namespace Domain
{
    public class RoleUseCases : Entity
    {
        // treba id role i id use case-a

        // Koga referencira RoleUseCases?
        // - Role
        // - UseCases

        // Ko referencira RoleUseCases?
        // - Niko


        public long UseCasesId { get; set; }
        public virtual UseCases UseCases { get; set; }
        public long RoleId { get; set; }
        public virtual Role Role { get; set; }

    }
}
