using Data.Access;
using Data.Access.Seeders;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeederController : ControllerBase
    {
        [HttpGet]
        public IActionResult Seed([FromServices] ApplicationDbContext _context)
        {

            DbSeeder dbSeeder = new DbSeeder(_context);

            return Ok();
        }

        [HttpDelete]
        public IActionResult ClearDbRecords([FromServices] ApplicationDbContext _context)
        {


            // Da li Neko referencira ROleUseCases?
            // - Ne
            _context.RoleUseCases.RemoveRange(_context.RoleUseCases);
            _context.BadgePosts.RemoveRange(_context.BadgePosts);


            // Da li je redosled dobar?
            // Prvo brisem zapise koje nemaju ka nikome zavisnost

            // Da li neko referencira Posts? Mislim da ne Post referencira User-a samo 
            _context.Posts.RemoveRange(_context.Posts);
            _context.Users.RemoveRange(_context.Users);

            _context.UseCases.RemoveRange(_context.UseCases);
            _context.Badges.RemoveRange(_context.Badges);
            _context.Roles.RemoveRange(_context.Roles);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
