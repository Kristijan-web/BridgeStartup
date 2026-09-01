using Application.DTO.PostApplication;

namespace Application.Queries.PostApplications
{


    public interface IPostApplicationQuery : IQuery<PostApplicationFilterDTO, PostsApplicationDbDTO>
    {
    }
}
