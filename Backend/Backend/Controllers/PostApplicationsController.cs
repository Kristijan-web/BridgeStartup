using Application.Commands.PostApplications;
using Application.DTO.PostApplication;
using Application.Queries.PostApplications;
using Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostApplicationsController : ControllerBase
    {
        // Napravi metodu za dohvatanje svih post-ova

        public UseCaseHandler _handler;
        public PostApplicationsController(UseCaseHandler handler)
        {

            _handler = handler;
        }

        [HttpGet]

        public IActionResult PostsApplications([FromServices] IPostsApplicationQuery query, [FromQuery] PostsApplicationFilterDTO filters)
        {

            return Ok(_handler.ExecuteQuery(query, filters));
        }

        // Ispod se dohvata 1 query zato mora 2 parametra da se proslede
        [HttpGet("{PostId}/{UserId}")]
        public IActionResult PostApplication([FromServices] IPostApplicationQuery query, [FromRoute] PostApplicationFilterDTO filters)
        {

            return Ok(_handler.ExecuteQuery(query, filters));
        }

        [HttpDelete("{postId}/{userId}")]
        public IActionResult DeletePostApplication([FromServices] IDeletePostApplicationCommand cmd, [FromRoute] DeletePostApplicationDTO dto)
        {
            _handler.ExecuteCommand(cmd, dto);
            return NoContent();
        }




    }
}
