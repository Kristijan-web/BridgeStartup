using Application.DTO.Post;
using Domain;

namespace Application.Queries.Posts
{
    // ovde mora da se vrati DTO, zbog object cycle problema
    // Sta je object cycle?
    // - To je problem koji nastaje kada pokusavamo da uradimo serijalizaciju u JSON, objekti drze rekurzivnu referencu jedan prema drugog
    public interface IPostsQuery : IQuery<PostsFilterDTO, IEnumerable<PostsResponseDTO>>
    {

    }
}
