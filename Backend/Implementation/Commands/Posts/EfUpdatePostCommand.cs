using Application.Commands.Posts;
using Application.DTO.Post;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Commands.Posts
{
    public class EfUpdatePostCommand : IUpdatePostCommand
    {
        private readonly ApplicationDbContext _context;

        public EfUpdatePostCommand(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Id => "update-post";

        public string Name => "update post";

        public void Execute(UpdatePostDTO dto)
        {
            // Dohvatamo post zajedno sa njegovim trenutnim badge-evima
            Post post = _context.Posts.Where(x => x.Id == dto.PostId).Include(x => x.BadgePosts).FirstOrDefault();


            // Kaze mi da je post null


            // Update-ujemo samo vrednosti koje su prosleđene

            if (dto.Title != null)
            {
                post.Title = dto.Title;
            }

            if (dto.Description != null)
            {
                post.Description = dto.Description;
            }

            if (dto.Email != null)
            {
                post.Email = dto.Email;
            }

            if (dto.Phone != null)
            {
                post.Phone = dto.Phone;
            }


            // Ako su poslati badge-evi
            if (dto.Badges.Any())
            {
                // Brišemo trenutne veze Post <-> Badge
                _context.BadgePosts.RemoveRange(post.BadgePosts);

                // Dohvatamo badge-eve koje je korisnik poslao
                List<Badge> badges = _context.Badges
                    .Where(x => dto.Badges.Contains(x.Name))
                    .ToList();

                // Pravimo nove veze
                foreach (Badge badge in badges)
                {
                    Badge_Post badgePost = new Badge_Post
                    {
                        PostId = post.Id,
                        BadgeId = badge.Id
                    };

                    _context.BadgePosts.Add(badgePost);
                }
            }

            _context.SaveChanges();
        }
    }
}