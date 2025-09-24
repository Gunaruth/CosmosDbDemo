using CosmosDbDemo.Interface;
using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using CosmosDbDemo.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDbDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        #region Declaration

        private readonly IUserRepository _repository;
        private const string containerName = "usersContainer";
        #endregion

        #region Constructor
        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region Get Method
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync(containerName));
        #endregion

        #region Add UserDetail
        /// <summary>
        /// AddUserDetail
        /// </summary>
        /// <param name="userReques"></param>
        /// <returns></returns>
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUserDetail(GavUser userRequest)
        {
            var userResponse = await _repository.AddAsync(containerName,userRequest);
            return CreatedAtAction(nameof(Get), new { id = userRequest.userId }, userRequest);
        }
        #endregion

        #region Update UserDetail
        /// <summary>
        /// UpdateUserDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUserDetail(GavUser userRequest)
        {
            var updated = await _repository.UpdateAsync(containerName,userRequest.id,userRequest);
            return updated == null ? NotFound() : Ok(updated);
        }
        #endregion

        #region GetUserDetailsById
        /// <summary>
        /// GetUserDetailsById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{id}/{userId}")]
        public async Task<IActionResult> GetUserDetailsById(string id, string userId)
        {
            var item = await _repository.GetByIdAsync(containerName, id, userId);
            return item == null ? NotFound() : Ok(item);
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// DeleteUser
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpDelete("{id}/{userId}")]
        public async Task<IActionResult> DeleteUser(string id, string userId)
        {
            bool isDeleted = await _repository.DeleteAsync(containerName, id, userId);

            if (isDeleted)
            {
                return Ok(new { message = "User deleted successfully" });
            }
            else
            {
                return NotFound(new { message = "User not found" });
            }
        }
        #endregion

        #region Get Username
        /// <summary>
        /// GetByUsername
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var users = await _repository.GetUsersByUsernameAsync(username);
            return Ok(users);
        }
        #endregion

        #region Get Region
        /// <summary>
        /// GetByRegion
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpGet("by-region/{region}")]
        public async Task<IActionResult> GetByRegion(string region)
        {
            var users = await _repository.GetUsersByRegionAsync(region);
            return Ok(users);
        }
        #endregion

        #region Get Role
        /// <summary>
        /// GetByRole
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet("by-role/{role}")]
        public async Task<IActionResult> GetByRole(string role)
        {
            var users = await _repository.GetUsersByEngagementRoleAsync(role);
            return Ok(users);
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
            var users = await _repository.GetUsersByEngagementStatusAsync(status);
            return Ok(users);
        }
        #endregion
    }
}
