using Application.Commands.PostApplications;
using Application.DTO.PostApplication;
using Application.DTO.PostApplication.Command;
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

        // Sada update

        // Da li za ovaj update da koristim put ili patch? PUT je ako update-ujem ceo objekat, ako update-ujem deo onda je PATCH
        [HttpPatch("{PostId}/{UserId}")]

        public IActionResult UpdatePostApplication([FromServices] IUpdatePostApplicationCommand cmd, long PostId, long UserId, [FromBody] PostApplicationUpdateDTO body)
        {


            // ma necu dozvoliti update-ovanje vec submitovane prijave na post

            // Nema smisla, update fajla treba da bude ponovno uploadovanje slike 

            PostApplicationUpdateDTO dto = new PostApplicationUpdateDTO
            {
                FilePath = body.FilePath,
                UserId = UserId,
                PostId = PostId
            };


            _handler.ExecuteCommand(cmd, dto);

            return NoContent();
        }



    }
}
