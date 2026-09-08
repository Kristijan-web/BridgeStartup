using Application.DTO.Post;

namespace Application.Commands.Posts
{
    public interface ICreatePostCommand : ICommand<CreatePostDTO>
    {
    }
}
