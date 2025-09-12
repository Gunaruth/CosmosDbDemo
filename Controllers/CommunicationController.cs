using CosmosDbDemo.Interface;
using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDbDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunicationController : ControllerBase
    {
        private readonly IRepository<CommunicationDetails> _communicationRepository;

        public CommunicationController(IRepository<CommunicationDetails> communicationRepository)
        {
            _communicationRepository = communicationRepository;
        }

        // GET: api/Communication
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommunicationDetails>>> GetAll()
        {
            var result = await _communicationRepository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Communication/{id}/{chatId}
        [HttpGet("{id}/{chatId}")]
        public async Task<IActionResult> GetById(string id, string chatId)
        {
            var result = await _communicationRepository.GetByIdAsync(id, chatId);
            if (result == null)
                return NotFound($"Document with id '{id}' and chatId '{chatId}' not found.");

            return Ok(result);
        }

        // POST: api/Communication
        [HttpPost]
        public async Task<IActionResult> Add(CommunicationDetails chat)
        {
            var result = await _communicationRepository.AddAsync(chat);
            //return CreatedAtAction(nameof(GetById), new { id = result.id, chatId = result.chatId }, result);
            return Ok(result);
        }

        // PUT: api/Communication/{chatId}
        [HttpPut("{chatId}")]
        public async Task<IActionResult> Update(string chatId, CommunicationDetails chat)
        {
            var result = await _communicationRepository.UpdateAsync(chatId, chat);

            if (result == null)
                return NotFound($"Unable to update: document with chatId '{chatId}' not found.");

            return Ok(result);
        }

        // DELETE: api/Communication/{id}/{chatId}
        [HttpDelete("{id}/{chatId}")]
        public async Task<IActionResult> Delete(string id, string chatId)
        {
            var deleted = await _communicationRepository.DeleteAsync(id, chatId);
            if (!deleted)
                return NotFound($"Document with id '{id}' and chatId '{chatId}' not found.");

            return NoContent();
        }
    }
}
