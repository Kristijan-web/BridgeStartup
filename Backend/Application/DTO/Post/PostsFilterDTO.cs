namespace Application.DTO.Post
{
    public class PostsFilterDTO
    {
        // Sta mi je potrebno za filtraciju?
        // Po kojim kolona cu dozvoliti filtraciju?

        // - po title-u
        public string? Title { get; set; }

        // Ovo moze biti niz stringova

        public List<string> Badge { get; set; } = new List<string>();
    }
}
