namespace Backend.JWT
{
    public class MakeJwtDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string Role { get; set; }

        IEnumerable<string> AllowedUseCases { get; set; } = new List<string>();



    }


}
