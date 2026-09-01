using Application.Commands.PostApplications;
using Application.DTO.PostApplication.Command;
using Data.Access;
using Domain;

namespace Implementation.Commands.PostApplications
{
    public class EfUpdatePostApplicationCommand : IUpdatePostApplicationCommand
    {
        public string Id => "update-post-application";

        public string Name => "update post application";

        public ApplicationDbContext _context;

        public EfUpdatePostApplicationCommand(ApplicationDbContext context)
        {

            _context = context;

        }

        public void Execute(PostApplicationUpdateDTO dto)
        {

            PostApplication postApplication = _context.PostApplications.First(x => x.UserId == dto.UserId && x.PostId == dto.PostId);





            postApplication.FilePath = dto.FilePath;

            _context.SaveChanges();




        }
    }
}
