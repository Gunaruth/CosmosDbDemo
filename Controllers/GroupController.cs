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
        private readonly IGroupRepository _repository;
        private const string containerName = "groupContainer";
        public GroupController(IGroupRepository repository)
        {
            _repository = repository;
        }
        #region Get All Group Details
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllGroups")]
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync(containerName));
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
            var userResponse = await _repository.AddAsync(containerName,auditEngagement);
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
            var updated = await _repository.UpdateAsync(containerName, auditEngagement.id, auditEngagement);
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
            var item = await _repository.GetByIdAsync(containerName, id, engagementId);
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
            bool isDeleted = await _repository.DeleteAsync(containerName, id, engagementId);

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

        #region Get Status
        /// <summary>
        /// GetByStatus
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var temp = await _repository.GetAllAsync(containerName);
            var result = await _repository.GetByStatusAsync(status);
            return Ok(result);
        }
        #endregion


        [HttpGet("by-createdby/{email}")]
        public async Task<IActionResult> GetByCreatedBy(string email)
        {
            var result = await _repository.GetByCreatedByAsync(email);
            return Ok(result);
        }

        [HttpGet("by-component-status/{componentStatus}")]
        public async Task<IActionResult> GetByComponentStatus(string componentStatus)
        {
            var result = await _repository.GetByComponentStatusAsync(componentStatus);
            return Ok(result);
        }

        [HttpGet("by-team-role/{role}")]
        public async Task<IActionResult> GetByTeamRole(string role)
        {
            var result = await _repository.GetByTeamRoleAsync(role);
            return Ok(result);
        }

        [HttpGet("by-location")]
        public async Task<IActionResult> GetByLocation([FromQuery] double lat, [FromQuery] double lng)
        {
            var result = await _repository.GetByLocationAsync(lat, lng);
            return Ok(result);
        }

    }
}
