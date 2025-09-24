using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDbDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ComponentController : ControllerBase
    {
        private readonly IComponentRepository _repository;
        private const string containerName = "componentContainer";
        public ComponentController(IComponentRepository repository)
        {
            _repository = repository;
        }
        #region Get All Components
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllComponents")]
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync(containerName));
        #endregion

        #region Add Component
        /// <summary>
        /// AddComponent
        /// </summary>
        /// <param name="componentEngagement"></param>
        /// <returns></returns>
        [HttpPost("AddComponent")]
        public async Task<IActionResult> AddComponent(ComponentEngagement componentEngagement)
        {
            var userResponse = await _repository.AddAsync(containerName,componentEngagement);
            return CreatedAtAction(nameof(Get), new { id = componentEngagement.id }, componentEngagement);
        }
        #endregion

        #region UpdateComponent
        /// <summary>
        /// UpdateComponent
        /// </summary>
        /// <param name="componentEngagement"></param>
        /// <returns></returns>

        [HttpPut("UpdateComponent")]
        public async Task<IActionResult> UpdateComponent(ComponentEngagement componentEngagement)
        {
            var updated = await _repository.UpdateAsync(containerName, componentEngagement.id, componentEngagement);
            return updated == null ? NotFound() : Ok(updated);
        }
        #endregion

        #region GetComponentDetailsById
        /// <summary>
        /// GetComponentById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engagementId"></param>
        /// <returns></returns>
        [HttpGet("{id}/{engagementId}")]
        public async Task<IActionResult> GetComponentById(string id, string engagementId)
        {
            var item = await _repository.GetByIdAsync(containerName, id, engagementId);
            return item == null ? NotFound() : Ok(item);
        }
        #endregion

        #region Delete Component
        /// <summary>
        /// DeleteComponent
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engagementId"></param>
        /// <returns></returns>

        [HttpDelete("{id}/{engagementId}")]
        public async Task<IActionResult> DeleteComponent(string id, string engagementId)
        {
            bool isDeleted = await _repository.DeleteAsync(containerName, id, engagementId);

            if (isDeleted)
            {
                return Ok(new { message = "Component deleted successfully" });
            }
            else
            {
                return NotFound(new { message = "Component not found" });
            }
        }
        #endregion


        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _repository.GetByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("by-createdby/{email}")]
        public async Task<IActionResult> GetByCreatedBy(string email)
        {
            var result = await _repository.GetByCreatedByAsync(email);
            return Ok(result);
        }

        [HttpGet("by-team-user/{userEmail}")]
        public async Task<IActionResult> GetByTeamUser(string userEmail)
        {
            var result = await _repository.GetByTeamUserAsync(userEmail);
            return Ok(result);
        }

        [HttpGet("by-engagement-type/{type}")]
        public async Task<IActionResult> GetByEngagementType(string type)
        {
            var result = await _repository.GetByEngagementTypeAsync(type);
            return Ok(result);
        }

        [HttpGet("by-opinion-id/{opinionId}")]
        public async Task<IActionResult> GetByOpinionId(string opinionId)
        {
            var result = await _repository.GetByOpinionIdAsync(opinionId);
            return Ok(result);
        }

    }
}
