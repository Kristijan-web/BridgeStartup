namespace Application.DTO.Post
{
    public class UpdatePostDTO
    {


        public long PostId { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        // Badg-eve, da sta ako update-uje vise badg-eva, neka onda podatak stigne kao niz
        public IEnumerable<string> Badges { get; set; } = new List<string>();

    }
}
