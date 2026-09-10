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

        [HttpDelete("{id}")]

        public IActionResult DeletePost([FromServices] IDeleteContactCommand cmd, long id)
        {
            _handler.ExecuteCommand(cmd, id);

            return NoContent();

        }



    }
}
