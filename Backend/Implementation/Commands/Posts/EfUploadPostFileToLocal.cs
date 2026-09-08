using Application.Commands;
using Application.DTO.Post;

namespace Implementation.Commands.Posts
{
    public class EfUploadPostFileToLocal : IUploadPostFileToCommand
    {
        public string Id => "apply-to-post-locally";

        public string Name => "Uploading post data locally";

        public async void Execute(ApplyToPostDTO dto)
        {
            Guid guid = Guid.NewGuid();

            string fileExtension = Path.GetExtension(dto.FileName);
            string originalFileName = Path.GetFileNameWithoutExtension(dto.FileName);

            string fileName =
                $"{originalFileName.ToLower()}-{guid}{fileExtension}";

            string filePath = Path.Combine("wwwroot", "CVs", fileName);

            using FileStream stream = new FileStream(
                filePath,
                FileMode.Create
            );

            await dto.FileStream.CopyToAsync(stream);
        }
    }
}
