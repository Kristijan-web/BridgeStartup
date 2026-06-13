using Data.Access;
using Data.Access.Seeders;
using Microsoft.AspNetCore.Http;
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
    }
}
