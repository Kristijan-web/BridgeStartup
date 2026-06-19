using Application.Commands;
using Application.DTO.Auth;
using Application.Queries;
using ASPLAB2.API.JWT;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost("register")]

        public IActionResult Register([FromServices] IRegisterUserCommand registerCommand, RegisterUserDTO dto)
        {


            registerCommand.Execute(dto);


            return Created();

        }


        [HttpPost("login")]
        public IActionResult Login([FromServices] ILoginQuery loginQuery, [FromServices] JwtHandler jwt, LoginDTO dto)
        {

            User userData = loginQuery.Execute(dto);



            if (!BCrypt.Net.BCrypt.Verify(dto.Password, userData.Password))
            {
                return Unauthorized();
            }



            var jwtToken = jwt.MakeToken(userData);

            // Kod ispod stavlja jwt token u http-only kolacic
            //Response.Cookies.Append("jwt", jwtToken, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.None,
            //    Expires = DateTimeOffset.UtcNow.AddSeconds(600)
            //});

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO
            {
                Id = userData.Id,
                Username = userData.Username,
                Email = userData.Email,
                Role = userData.Role.Name
            };

            return Ok(new
            {
                user = loginResponseDTO,
                token = jwtToken
            });



        }


    }
}
