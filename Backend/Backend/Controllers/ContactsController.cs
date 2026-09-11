using Backend.Authorization;
using Data.Access;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Application.Commands.Contacts;
using Application.DTO.Contact;
using Application.Queries.Contacts;
using Implementation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {

        private UseCaseHandler _handler;

        public ContactsController(UseCaseHandler handler)
        {

            _handler = handler;

        }

        // Fali getContact
        // Fali Delete Contact

        [HttpGet]

        public IActionResult GetAllContacts([FromServices] IGetAllContactsQuery query, [FromQuery] FilterContactsDTO dto)
        {

            return Ok(_handler.ExecuteQuery(query, dto));
        }

        [HttpGet("{id}")]
        public IActionResult GetContact([FromServices] IGetContactQuery query, long id)
        {

            return Ok(_handler.ExecuteQuery(query, id));
        }


        [HttpPost]

        public IActionResult CreatePost([FromServices] ICreateContactCommand cmd, [FromBody] CreateContactDTO dto)
        {

            _handler.ExecuteCommand(cmd, dto);

            return Created();

        }

        [HttpPut("{id:long}")]
        [Authorize]
        [ServiceFilter(typeof(AdminAccessFilter))]
        public async Task<IActionResult> UpdateContact(long id, [FromBody] CreateContactDTO dto,
            [FromServices] ApplicationDbContext context)
        {
            var contact = await context.Contacts.SingleOrDefaultAsync(x => x.Id == id);
            if (contact == null) return NotFound(new { message = "Contact not found." });
            if (!await context.Users.AnyAsync(x => x.Id == dto.UserId))
                return BadRequest(new { message = "Select an existing sender." });

            contact.UserId = dto.UserId;
            contact.Subject = dto.Subject.Trim();
            contact.Message = dto.Message.Trim();
            contact.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]

        public IActionResult DeletePost([FromServices] IDeleteContactCommand cmd, long id)
        {
            _handler.ExecuteCommand(cmd, id);

            return NoContent();

        }



    }
}
