using Application.Commands.Users;
using Application.DTO.User;
using Application.Queries;
using Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        public UseCaseHandler _handler;
        public UsersController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        public IActionResult GetUsers([FromServices] IUsersQuery query, [FromQuery] SearchUsersDTO dto)
        {


            return Ok(_handler.ExecuteQuery(query, dto));

        }

        // 
        [HttpGet("{id}")]
        public IActionResult GetUser(int id, [FromServices] IUserQuery query)
        {



            return Ok(_handler.ExecuteQuery(query, id));

        }

        [HttpGet("{id}/getPosts")]

        // Sta je sve potrebno 
        // - Handler
        // - Query
        // - Id
        public IActionResult getUserPosts([FromServices] IGetUserPostsQuery query, int id)
        {



            return Ok(_handler.ExecuteQuery(query, id));

        }

        [HttpDelete("{id}")]

        public IActionResult DeleteUser([FromServices] IDeleteUserCommand cmd, int id)
        {

            _handler.ExecuteCommand(cmd, id);
            return NoContent();
        }

    }
}

// MyPosts
