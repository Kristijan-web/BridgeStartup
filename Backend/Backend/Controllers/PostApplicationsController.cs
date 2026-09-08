using Application.Commands.PostApplications;
using Application.DTO.Post;
using Application.DTO.PostApplication;
using Application.Queries.PostApplications;
using Application.Queries.Posts;
using Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostApplicationsController : ControllerBase
    {
        // Napravi metodu za dohvatanje svih post-ova

        public UseCaseHandler _handler;
        public PostApplicationsController(UseCaseHandler handler)
        {

            _handler = handler;
        }

        [HttpGet]

        public IActionResult PostsApplications([FromServices] IPostsApplicationQuery query, [FromQuery] PostsApplicationFilterDTO filters)
        {

            return Ok(_handler.ExecuteQuery(query, filters));
        }

        // Ispod se dohvata 1 query zato mora 2 parametra da se proslede
        [HttpGet("{PostId}/{UserId}")]
        public IActionResult PostApplication([FromServices] IPostApplicationQuery query, [FromRoute] PostApplicationFilterDTO filters)
        {

            return Ok(_handler.ExecuteQuery(query, filters));
        }

        [HttpGet("{postId:long}/{userId:long}/file")]

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult DownloadFile(long postId, long userId, [FromServices] IApplicationFileQuery query)
        {
            var file = _handler.ExecuteQuery(query, new PostApplicationFilterDTO { PostId = postId, UserId = userId });
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return File(file.Content, file.ContentType, file.FileName);
        }

        [HttpDelete("{postId}/{userId}")]
        public IActionResult DeletePostApplication([FromServices] IDeletePostApplicationCommand cmd, [FromRoute] DeletePostApplicationDTO dto)
        {
            _handler.ExecuteCommand(cmd, dto);
            return NoContent();
        }

        // Kako dohvatam prijave od korisnika koji je objavio post?
        // Sta mi je potrebno da bih dohvatio prijave od korisnika koji je objavio odredjeni post?
        // - Prijave za post su u tabelu PostAPplication -> treba mi id post-a i tu cu videti sve applikacije, kako cu naci onoga ko je objavio post
        // - Sam post je u tabeli post

        [HttpGet("{postId}/{userId}/applied")]
        public IActionResult GetPostApplicationsForOwner([FromServices] IGetPostApplicationsForOwnerQuery query,
         [FromRoute] long postId,
         [FromRoute] long userId)
        {
            // postId -> ID posta
            // userId -> ID korisnika za kog proveravaš da li je owner

            var dto = new GetPostApplicationsForOwnerQueryDTO
            {
                PostId = postId,
                UserId = userId
            };

            return Ok(_handler.ExecuteQuery(query, dto));
        }





    }
}
