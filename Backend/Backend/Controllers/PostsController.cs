using Application.DTO.Post;
using Application.Queries;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {

        // Koji response kod ce vratiti jwt autorizacija i da li ce se ona razlikovati od tipa neispravnog jwt-a?
        // - nesto od 403 ili 404
        // Koju poruku ce vratiti jwt autorizacjia

        // Kako bi isla sintaksa da hocu da dopustim samo one cija je role-a "user"

        [Authorize(Roles = "user")]

        [HttpGet]

        public IActionResult GetAllPosts(IPostsQuery query, [FromQuery] PostsDTO dto)
        {

            // prosledjuje se dto a dto nije ni poslat, OVO JE PROBLEM
            IEnumerable<Post> posts = query.Execute(dto);

            IEnumerable<PostsResponseDTO> postsDTO = posts.Select(x => new PostsResponseDTO
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Email = x.Email,
                Phone = x.Phone,

                User = new UserDTO
                {
                    Username = x.User.Username,
                    Email = x.User.Email
                }
            });



            return Ok(postsDTO);

        }


    }
}
