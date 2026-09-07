namespace Application.DTO.User
{

    // Parametri po kojima je dozvoljeno search-anje user-a
    public class SearchUsersDTO
    {
        // Po email-u

        public string? Username { get; set; }
        public int? Page { get; set; }
        public string? sortBy { get; set; }

        public string? sortOrder { get; set; }
    }

}
