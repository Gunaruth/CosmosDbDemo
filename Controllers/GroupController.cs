using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDbDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IRepository<AuditEngagement> _repository;
        public GroupController(IRepository<AuditEngagement> repository)
        {
            _repository = repository;
        }
        #region Get All Group Details
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllGroups")]
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync());
        #endregion

        #region Add Group
        /// <summary>
        /// AddGroup
        /// </summary>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        [HttpPost("AddGroup")]
        public async Task<IActionResult> AddGroup(AuditEngagement auditEngagement)
        {
            var userResponse = await _repository.AddAsync(auditEngagement);
            return CreatedAtAction(nameof(Get), new { id = auditEngagement.id }, auditEngagement);
        }
        #endregion

        #region Update Group Details
        /// <summary>
        /// UpdateGroupDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>

        [HttpPut("UpdateGroup")]
        public async Task<IActionResult> UpdateGroup(AuditEngagement auditEngagement)
        {
            var updated = await _repository.UpdateAsync(auditEngagement.id, auditEngagement);
            return updated == null ? NotFound() : Ok(updated);
        }
        #endregion

        #region GetGroupDetailsById
        /// <summary>
        /// GetGroupDetailsById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engagementId"></param>
        /// <returns></returns>
        [HttpGet("{id}/{engagementId}")]
        public async Task<IActionResult> GetGroupById(string id, string engagementId)
        {
            var item = await _repository.GetByIdAsync(id, engagementId);
            return item == null ? NotFound() : Ok(item);
        }
        #endregion

        #region DeleteGroup
        /// <summary>
        /// DeleteGroup
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engagementId"></param>
        /// <returns></returns>

        [HttpDelete("{id}/{engagementId}")]
        public async Task<IActionResult> DeleteGroup(string id, string engagementId)
        {
            bool isDeleted = await _repository.DeleteAsync(id, engagementId);

            if (isDeleted)
            {
                return Ok(new { message = "Group deleted successfully" });
            }
            else
            {
                return NotFound(new { message = "Group not found" });
            }
        }
        #endregion
    }
}
