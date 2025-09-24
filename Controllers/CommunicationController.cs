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
        private readonly ICommunicationRepository _communicationRepository;
        private const string containerName = "componentContainer";
        public CommunicationController(ICommunicationRepository communicationRepository)
        {
            _communicationRepository = communicationRepository;
        }

        // GET: api/Communication
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommunicationDetails>>> GetAll()
        {
            var result = await _communicationRepository.GetAllAsync(containerName);
            return Ok(result);
        }

        // GET: api/Communication/{id}/{chatId}
        [HttpGet("{id}/{chatId}")]
        public async Task<IActionResult> GetById(string id, string chatId)
        {
            var result = await _communicationRepository.GetByIdAsync(containerName,id, chatId);
            if (result == null)
                return NotFound($"Document with id '{id}' and chatId '{chatId}' not found.");

            return Ok(result);
        }

        // POST: api/Communication
        [HttpPost]
        public async Task<IActionResult> Add(CommunicationDetails chat)
        {
            var result = await _communicationRepository.AddAsync(containerName, chat);
            //return CreatedAtAction(nameof(GetById), new { id = result.id, chatId = result.chatId }, result);
            return Ok(result);
        }

        // PUT: api/Communication/{chatId}
        [HttpPut("{chatId}")]
        public async Task<IActionResult> Update(string chatId, CommunicationDetails chat)
        {
            var result = await _communicationRepository.UpdateAsync(containerName, chatId, chat);

            if (result == null)
                return NotFound($"Unable to update: document with chatId '{chatId}' not found.");

            return Ok(result);
        }

        // DELETE: api/Communication/{id}/{chatId}
        [HttpDelete("{id}/{chatId}")]
        public async Task<IActionResult> Delete(string id, string chatId)
        {
            var deleted = await _communicationRepository.DeleteAsync(containerName, id, chatId);
            if (!deleted)
                return NotFound($"Document with id '{id}' and chatId '{chatId}' not found.");

            return NoContent();
        }


        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _communicationRepository.GetByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("by-participant-role/{role}")]
        public async Task<IActionResult> GetByParticipantRole(string role)
        {
            var result = await _communicationRepository.GetByParticipantRoleAsync(role);
            return Ok(result);
        }

        [HttpGet("by-participant-user/{userId}")]
        public async Task<IActionResult> GetByParticipantUserId(string userId)
        {
            var result = await _communicationRepository.GetByParticipantUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("by-message-sender/{senderId}")]
        public async Task<IActionResult> GetByMessageSender(string senderId)
        {
            var result = await _communicationRepository.GetByMessageSenderAsync(senderId);
            return Ok(result);
        }

        [HttpGet("by-visible-to/{userId}")]
        public async Task<IActionResult> GetByVisibleToUser(string userId)
        {
            var result = await _communicationRepository.GetByVisibleToUserAsync(userId);
            return Ok(result);
        }

    }
}
