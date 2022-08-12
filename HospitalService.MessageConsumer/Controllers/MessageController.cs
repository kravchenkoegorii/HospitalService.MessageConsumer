using HospitalService.MessageConsumer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HospitalService.MessageConsumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageConsumerRepository _messageConsumer;

        public MessageController(IMessageConsumerRepository messageConsumer)
        {
            _messageConsumer = messageConsumer;
        }

        /// <summary>
        /// Returns message by ID.
        /// </summary>
        /// <param name="id">ID of the message</param>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Returns message by ID.",
            Description = "Returns message by ID.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync(int id)
        {
            return Ok(await _messageConsumer.GetMessage(id));
        }

        /// <summary>
        /// Returns list of all the messages in database.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Returns list of all the messages in database.",
            Description = "Returns list of all the messages in database.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _messageConsumer.GetMessages());
        }
    }
}
