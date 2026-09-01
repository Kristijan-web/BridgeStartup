using Application.DTO.PostApplication.Command;

namespace Application.Commands.PostApplications
{
    // Update je DTO
    public interface IUpdatePostApplicationCommand : ICommand<PostApplicationUpdateDTO>
    {
    }
}
