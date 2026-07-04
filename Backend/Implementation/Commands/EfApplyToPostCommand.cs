using Application.Commands;
using Application.DTO.Post;
using Data.Access;

namespace Implementation.Commands
{
    public class EfApplyToPostCommand : IApplyToPostCommand
    {

        public string Id => "apply-to-post";

        public string Name => "applying to post";

        ApplicationDbContext _context;

        public EfApplyToPostCommand(ApplicationDbContext context)
        {
            _context = context;

        }

        public void Execute(PostApplyUserDTO dto)
        {
            // logika za upis u bazu

            // treba da se uradi validacija za ovu logiku, MORA VALIDATOR
        }

    }
}
