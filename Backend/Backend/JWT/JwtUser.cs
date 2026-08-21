using Application;

namespace Backend.JWT
{
    public class JwtUser : IApplicationUser // potpis -> kako izgleda objekat user-a iz jwt-a
    {
        // obican dto
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public IEnumerable<string> AllowedUseCases { get; set; } = new List<string>();
    }
}
