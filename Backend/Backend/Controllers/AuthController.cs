using Application.Commands;
using Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public IActionResult Register([FromServices] IRegisterUserCommand registerCommand, RegisterUserDTO dto)
        {
            // treba DTO od korisnika

            registerCommand.Execute(dto);

            return Ok();

        }
    }
}
