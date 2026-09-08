namespace Application.Queries.Posts;

public record BadgeChoiceDTO(long Id, string Name);
public interface IBadgesQuery : IQuery<int, IEnumerable<BadgeChoiceDTO>> { }

