using Application.Commands;
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

            return Ok(_handler.ExecuteQuery(query, dto));

        }

        [HttpGet("{id}")]

        public IActionResult GetPost(int id, [FromServices] IPostQuery query)
        {

            return Ok(_handler.ExecuteQuery(query, id));

        }
        // kako ide sintaksa da ruta bude /posts/apply

        [HttpPost("apply")]

        public IActionResult ApplyToPost([FromServices] UseCaseHandler _handler, [FromServices] IUploadPostFileToCommand cmd, [FromForm] PostApplyDTO dto)
        {


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




    }
}
