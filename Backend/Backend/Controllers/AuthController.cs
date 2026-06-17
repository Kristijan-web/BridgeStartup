using Application.Commands;
using Application.DTO;
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

        // Koja ce biti Http metoda za login?
        // - GET

        [HttpGet]
        public IActionResult Login([FromServices] ILoginQuery loginQuery, [FromServices] JwtHandler jwt, LoginDTO dto)
        {

            // Dodaj hash-ovanje korisnikovog passworda

            LoginResponseDTO userData = loginQuery.Execute(dto);


            User user = new User
            {
                Username = userData.Username,
                Email = userData.Email,
                Id = userData.Id,
            };

            var jwtToken = jwt.MakeToken(user);

            // Kod ispod stavlja jwt token u http-only kolacic
            //Response.Cookies.Append("jwt", jwtToken, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.None,
            //    Expires = DateTimeOffset.UtcNow.AddSeconds(600)
            //});

            return Ok(new
            {
                token = jwtToken
            });



        }


    }
}
