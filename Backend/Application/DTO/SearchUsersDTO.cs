namespace Application.DTO
{

    // Parametri po kojima je dozvoljeno search-anje user-a
    public class SearchUsersDTO
    {
        // Po email-u

        public string? Email { get; set; }
        public int? Page { get; set; }
    }
}
