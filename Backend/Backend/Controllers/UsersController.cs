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


        public IActionResult getUserPosts([FromServices] IGetUserPostsQuery query, int id)
        {



            return Ok(_handler.ExecuteQuery(query, id));

        }

        // Pravi update uzer-a ali bez mogucnosti update-a sifre
        [HttpPatch("{id}")]

        public IActionResult UpdateUser()
        {

            return NoContent();

        }

        [HttpDelete("{id}")]

        public IActionResult DeleteUser([FromServices] IDeleteUserCommand cmd, int id)
        {

            _handler.ExecuteCommand(cmd, id);
            return NoContent();
        }

    }
}


