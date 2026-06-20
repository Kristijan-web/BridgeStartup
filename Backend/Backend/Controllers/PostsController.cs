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

        [HttpGet("{id}")]

        public IActionResult GetPost(int id, [FromServices] IPostQuery query)
        {


            Post post = query.Execute(id);


            PostsResponseDTO postDTO = new PostsResponseDTO
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Email = post.Email,
                Phone = post.Phone,
                User = new UserDTO
                {
                    Username = post.User.Username,
                    Email = post.User.Email
                }

            };

            return Ok(postDTO);

        }


    }
}
