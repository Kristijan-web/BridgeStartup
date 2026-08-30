using Application.DTO.PostApplication;

namespace Application.Queries.PostApplications
{

    public interface IPostsApplicationQuery : IQuery<PostsApplicationFilterDTO, IEnumerable<PostsApplicationDbDTO>>
    {

    }
}
