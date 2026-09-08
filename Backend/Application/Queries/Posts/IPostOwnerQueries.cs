using Application.DTO.Post;
using Application.DTO.PostApplication;

namespace Application.Queries.Posts;

public interface IMyPostsQuery : IQuery<int, IEnumerable<OwnedPostDTO>> { }
public interface IPostApplicantsQuery : IQuery<long, IEnumerable<PostApplicantDTO>> { }
public interface IApplicationFileQuery : IQuery<PostApplicationFilterDTO, ApplicationFileDTO> { }

