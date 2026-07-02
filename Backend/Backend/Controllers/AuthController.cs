using Application.Commands;
using Application.DTO.Auth;
using Application.DTO.User;
using Application.Queries;
using ASPLAB2.API.JWT;
using Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        UseCaseHandler _handler;
        public AuthController(UseCaseHandler handler)
        {
            _handler = handler;

        }

        [HttpPost("register")]

        public IActionResult Register([FromServices] IRegisterUserCommand registerCommand, RegisterUserDTO dto)
        {



            _handler.ExecuteCommand(registerCommand, dto);

            return Created();

        }


        [HttpPost("login")]
        public IActionResult Login([FromServices] ILoginQuery loginQuery, [FromServices] JwtHandler jwt, LoginDTO dto)
        {


            UserDbDTO userData = _handler.ExecuteQuery(loginQuery, dto);

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, userData.Password))
            {
                return Unauthorized();
            }

            var jwtToken = jwt.MakeToken(userData);

            return Ok(new
            {
                user = userData,
                token = jwtToken
            });


        }


    }
}
