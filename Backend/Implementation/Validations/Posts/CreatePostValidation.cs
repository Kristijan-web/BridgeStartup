using Application.DTO.Post;
using Data.Access;
using FluentValidation;

namespace Implementation.Validators.Posts
{
    public class CreatePostValidation : AbstractValidator<CreatePostDTO>
    {
        private readonly ApplicationDbContext _context;

        public CreatePostValidation(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0.")
                .Must(UserExists)
                .WithMessage("User does not exist.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MinimumLength(3)
                .WithMessage("Title must contain at least 3 characters.")
                .MaximumLength(100)
                .WithMessage("Title cannot contain more than 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MinimumLength(10)
                .WithMessage("Description must contain at least 10 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email format is not valid.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone is required.");

            RuleFor(x => x.Badges)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Badges are required.")
                .NotEmpty()
                .WithMessage("At least one badge is required.")
                .Must(HaveUniqueBadges)
                .WithMessage("Duplicate badges are not allowed.")
                .Must(BadgesExist)
                .WithMessage("One or more badges do not exist.");

            RuleForEach(x => x.Badges)
                .GreaterThan(0)
                .WithMessage("Badge id must be greater than 0.");
        }

        private bool UserExists(long userId)
        {
            return _context.Users.Any(x => x.Id == userId);
        }

        private bool BadgesExist(IEnumerable<long> badgeIds)
        {
            var ids = badgeIds
                .Distinct()
                .ToList();

            var existingBadgesCount = _context.Badges
                .Count(x => ids.Contains(x.Id));

            return existingBadgesCount == ids.Count;
        }

        private bool HaveUniqueBadges(IEnumerable<long> badgeIds)
        {
            return badgeIds.Distinct().Count() == badgeIds.Count();
        }
    }
}