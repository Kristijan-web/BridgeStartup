using Application.Queries.Posts;
using Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BadgesController(UseCaseHandler handler) : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromServices] IBadgesQuery query) => Ok(handler.ExecuteQuery(query, 0));
}

