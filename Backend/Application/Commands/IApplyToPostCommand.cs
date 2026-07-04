using Application.DTO.Post;

namespace Application.Commands
{
    // komanda samo prima DTO
    public interface IApplyToPostCommand : ICommand<PostApplyUserDTO>
    {
    }
}
