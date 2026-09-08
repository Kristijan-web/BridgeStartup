using Application.DTO.User;
using Application.Queries;
using Domain;
using Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetUsers([FromServices] IUsersQuery query, [FromQuery] SearchUsersDTO dto)
        {

            IEnumerable<User> users = query.Execute(dto);


            IEnumerable<UserResponseDTO> usersDTO = users.Select(x => new UserResponseDTO
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                Role = x.Role.Name

            });


            return Ok(usersDTO);

        }

        // 
        [HttpGet("{id}")]
        public IActionResult GetUser(int id, [FromServices] IUserQuery query)
        {

            User user = query.Execute(id);


            UserResponseDTO userDTO = new UserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.Name
            };


            return Ok(userDTO);

        }

        [HttpGet("{id}/getPosts")]

        // Sta je sve potrebno 
        // - Handler
        // - Query
        // - Id
        public IActionResult getUserPosts(int id, [FromServices] UseCaseHandler _handler, [FromServices] IGetUserPostsQuery query) {



            return Ok(_handler.ExecuteQuery(query,id));
       
        }
    }
}

// MyPosts
