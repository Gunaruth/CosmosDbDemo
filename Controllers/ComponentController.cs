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
        private readonly IRepository<AuditEngagement> _repository;
        public ComponentController(IRepository<AuditEngagement> repository)
        {
            _repository = repository;
        }
        #region Get All Components
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllComponents")]
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync());
        #endregion

        #region Add Component
        /// <summary>
        /// AddComponent
        /// </summary>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        [HttpPost("AddComponent")]
        public async Task<IActionResult> AddComponent(AuditEngagement auditEngagement)
        {
            var userResponse = await _repository.AddAsync(auditEngagement);
            return CreatedAtAction(nameof(Get), new { id = auditEngagement.id }, auditEngagement);
        }
        #endregion

        #region UpdateComponent
        /// <summary>
        /// UpdateComponent
        /// </summary>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>

        [HttpPut("UpdateComponent")]
        public async Task<IActionResult> UpdateComponent(AuditEngagement auditEngagement)
        {
            var updated = await _repository.UpdateAsync(auditEngagement.id, auditEngagement);
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
            var item = await _repository.GetByIdAsync(id, engagementId);
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
            bool isDeleted = await _repository.DeleteAsync(id, engagementId);

            if (isDeleted)
            {
                return Ok(new { message = "Group deleted successfully" });
            }
            else
            {
                return NotFound(new { message = "User not found" });
            }
        }
        #endregion
    }
}
