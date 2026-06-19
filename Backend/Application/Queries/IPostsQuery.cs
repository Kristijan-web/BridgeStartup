using Application.DTO.Post;
using Domain;

namespace Application.Queries
{
    public interface IPostsQuery : IQuery<PostsDTO, IEnumerable<Post>>
    {

    }
}
