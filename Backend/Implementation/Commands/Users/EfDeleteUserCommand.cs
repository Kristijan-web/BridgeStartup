using Application.Commands.Users;
using Application.Exceptions;
using Data.Access;

namespace Implementation.Commands.Users
{
    public class EfDeleteUserCommand : IDeleteUserCommand
    {
        public string Id => "delete-user";

        public string Name => "delete user";

        private ApplicationDbContext _context;

        public EfDeleteUserCommand(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Execute(int id)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException("User nije pronadjen");
            }

            user.DeletedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }


    }
}
