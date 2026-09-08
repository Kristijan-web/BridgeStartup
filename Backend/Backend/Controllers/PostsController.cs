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
        [HttpPost("create")]

        public IActionResult CreatePost([FromServices] ICreatePostCommand cmd, CreatePostDTO dto)
        {

            _handler.ExecuteCommand(cmd, dto);

            return Created();

        }

        [HttpPost]


        public IActionResult ApplyToPost([FromServices] UseCaseHandler _handler, [FromServices] IUploadPostFileToCommand cmd, [FromForm] PostApplyDTO dto)
        {


            dto.UserId = long.Parse(User.FindFirst("Id")!.Value);
            ApplyToPostDTO postDTO = new ApplyToPostDTO
            {
                UserId = dto.UserId,
                PostId = dto.PostId,
                FileName = dto.userFile.FileName,
                ContentType = dto.userFile.ContentType,
                FileLength = dto.userFile.Length,
                FileStream = dto.userFile.OpenReadStream()

            };

            _handler.ExecuteCommand(cmd, postDTO);
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
