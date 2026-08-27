using Application.DTO.Post;
using Domain;

namespace Application.Queries.Posts
{
    public interface IPostQuery : IQuery<int, PostsResponseDTO>
    {

    }
}
