namespace Application.DTO.Post
{
    public class PostsFilterDTO
    {

        public string? Title { get; set; }

        public List<string> Badge { get; set; } = new List<string>();

        public IEnumerable<string> SortBy { get; set; } = new List<string>();

        public IEnumerable<string> SortOrder { get; set; } = new List<string>();

        public int? Page { get; set; }


    }
}
