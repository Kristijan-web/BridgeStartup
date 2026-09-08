using Application.Commands;
using Application.Commands.Posts;
using Application.DTO.Post;
using Application.Queries.Posts;
//using Domain;
using Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {

        private UseCaseHandler _handler;
        public PostsController(UseCaseHandler handler)
        {
            _handler = handler;
        }



        [HttpGet]

        public IActionResult GetAllPosts([FromServices] IPostsQuery query, [FromQuery] PostsFilterDTO dto)
        {

            // Ovde mora da se uradi filtiracija

            return Ok(query.Execute(dto));

        }

        [HttpGet("{id}")]

        public IActionResult GetPost(int id, [FromServices] IPostQuery query)
        {

            return Ok(query.Execute(id));

        }
        // kako ide sintaksa da ruta bude /posts/apply
        [HttpPost]

        public IActionResult CreatePost([FromServices] ICreatePostCommand cmd, CreatePostDTO dto)
        {

            _handler.ExecuteCommand(cmd, dto);

            return Created();

        }

        [HttpDelete("{id}")]

        public IActionResult DeletePost([FromServices] IDeletePostCommand cmd, int id)
        {

            _handler.ExecuteCommand(cmd, id);
            return NoContent();
        }

        [HttpPost("apply")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
        public async Task<IActionResult> ApplyToPost([FromServices] IUploadPostFileToCommand cmd,
            [FromForm] PostApplyDTO dto, CancellationToken cancellationToken)
        {
            await using var stream = dto.userFile.OpenReadStream();
            var application = new ApplyToPostDTO
            {
                UserId = long.Parse(User.FindFirst("Id")!.Value),
                PostId = dto.PostId,
                FileName = dto.userFile.FileName,
                ContentType = dto.userFile.ContentType,
                FileLength = dto.userFile.Length,
                FileStream = stream
            };
            await _handler.ExecuteCommandAsync(cmd, application, cancellationToken);
            return NoContent();
        }
        // Treba mi update post-a
        // Koju http metodu cu da koristim?
        // - Radim update onda je put ili patch
        // - Da li cu raditi delimicne update-ove objekta?
        // - Da -> onda je patch metoda

        // Iz rute mi treba id post-a koji update-ujem
        [HttpPatch("{PostId}")]


        // Update podaci dolaze iz body-a?
        // - Da
        public IActionResult UpdatePost([FromServices] IUpdatePostCommand cmd, [FromBody] UpdatePostDTO dto, [FromRoute] long PostId)
        {

            // Da li ce se id iz query string-a mapirati u DTO?

            dto.PostId = PostId;

            _handler.ExecuteCommand(cmd, dto);
            return NoContent();
        }









    }
}
