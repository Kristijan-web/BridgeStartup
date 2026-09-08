using Application;
using FluentValidation;
using Application.Commands.Posts;
using Application.DTO.Post;
using Data.Access;
using Domain;
using Implementation.Validators.Posts;

namespace Implementation.Commands.Posts
{
    public class EfCreatePostCommand : ICreatePostCommand
    {
        private readonly ApplicationDbContext _context;

        private CreatePostValidation _validator;
        private readonly IApplicationUser _user;

        public EfCreatePostCommand(ApplicationDbContext context, CreatePostValidation validator, IApplicationUser user)
        {
            _context = context;
            _validator = validator;
            _user = user;
        }

        public string Id => "create-post";

        public string Name => "Create post";

        public void Execute(CreatePostDTO dto)
        {



            dto.UserId = _user.Id;
            _validator.ValidateAndThrow(dto);



            // badgeve za post upisujem u tabelu badge_post, znaci i u toj tabelu treba da se uradi zapis, klijent prosledjuje id-eve badg-eva ["1","2","3"]
            var post = new Post
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Description = dto.Description,
                Email = dto.Email,
                Phone = dto.Phone,

                BadgePosts = dto.Badges.Select(badgeId => new Badge_Post
                {
                    BadgeId = badgeId
                }).ToHashSet()
            };

            _context.Posts.Add(post);
            _context.SaveChanges();
        }
    }
}
