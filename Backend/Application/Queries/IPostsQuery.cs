using Application.DTO;
using Domain;

namespace Application.Queries
{
    public interface IPostsQuery : IQuery<PostsDTO, IEnumerable<Post>>
    {

    }
}
