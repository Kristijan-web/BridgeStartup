using Application.DTO;
using Application.Queries;
using Domain;
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

            // Sada sve ovo mora da prebacim u DTO, al zasto odmah iz baze nisam vratio DTO

            IEnumerable<UserResponseDTO> usersDTO = users.Select(x => new UserResponseDTO
            {
                Username = x.Username,
                Email = x.Email,
                Role = x.Role.Name

            });


            return Ok(usersDTO);

        }
    }
}
