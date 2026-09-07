using Application.DTO.Post;

namespace Application.Commands.Posts
{

    // Treba mi dto za post
    public interface IUpdatePostCommand : ICommand<UpdatePostDTO>
    {
    }
}
