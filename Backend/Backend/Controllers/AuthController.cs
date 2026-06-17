using Application.Commands;
using Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost]
        public IActionResult Register([FromServices] IRegisterUserCommand registerCommand, RegisterUserDTO dto)
        {
            // treba DTO od korisnika

            registerCommand.Execute(dto);

            // Kako luka prosledjuje objekat za validaciju?
            // - Sigurno registruje klasu u DI container
            // - Ta klasa mora da se prosledi ode
            // Kako se onda klasa za validator prosledjuje execute-u metodi kada ona prima samo DTO
            // - Ne prosledjuje se, validator se poziva u controller-u!

            return Created();

        }
    }
}
