using Application.Commands.Users;
using Application.DTO.User;
using Application.Exceptions;
using Data.Access;

namespace Implementation.Commands.Users
{
    public class EfUpdateUserCommand : IUpdateUserCommand
    {
        private readonly ApplicationDbContext _context;

        public EfUpdateUserCommand(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Id => "update-user";

        public string Name => "update user";

        public void Execute(UpdateUserDTO dto)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Id == dto.UserId);

            if (user == null)
            {
                throw new EntityNotFoundException(
                    $"User with the id of {dto.UserId} not found."
                );
            }

            if (!String.IsNullOrWhiteSpace(dto.Username))
            {
                user.Username = dto.Username;
            }

            if (!String.IsNullOrWhiteSpace(dto.Email))
            {
                user.Email = dto.Email;
            }

            if (!String.IsNullOrWhiteSpace(dto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            user.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }
    }
}