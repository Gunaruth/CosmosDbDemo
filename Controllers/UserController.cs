using CosmosDbDemo.Interface;
using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDbDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        #region Declaration

        private readonly IRepository<GavUser> _repository;

        #endregion

        #region Constructor
        public UserController(IRepository<GavUser> repository)
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
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync());
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
            var userResponse = await _repository.AddAsync(userRequest);
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
            var updated = await _repository.UpdateAsync(userRequest.id,userRequest);
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
            var item = await _repository.GetByIdAsync(id, userId);
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
            bool isDeleted = await _repository.DeleteAsync(id, userId);

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
    }
}
