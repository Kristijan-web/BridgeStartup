using Application.Queries.Posts;
using Data.Access;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Posts;

public class EfBadgesQuery(ApplicationDbContext context) : IBadgesQuery
{
    public string Id => "get-post-badges";
    public string Name => "get available post badges";
    public IEnumerable<BadgeChoiceDTO> Execute(int unused) => context.Badges.AsNoTracking()
        .Where(badge => badge.DeletedAt == null).OrderBy(badge => badge.Name)
        .Select(badge => new BadgeChoiceDTO(badge.Id, badge.Name)).ToList();
}

