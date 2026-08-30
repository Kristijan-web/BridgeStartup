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

        public IActionResult GetAllPostApplications([FromServices] IPostsApplicationQuery query, [FromQuery] PostsApplicationFilterDTO filters)
        {

            return Ok(_handler.ExecuteQuery(query, filters));
        }



    }
}
