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

        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructor
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Get Method
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _userRepository.GetUsers());
        #endregion

        #region Add UserDetail
        /// <summary>
        /// AddUserDetail
        /// </summary>
        /// <param name="userReques"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GavUser> AddUserDetail(GavUser userReques)
        {
            GavUser userResponse = await _userRepository.AdduserDetail(userReques);
            return userResponse;
            //return CreatedAtAction(nameof(Get), new { id = userReques.userId }, userReques);
        }
        #endregion

        #region Update UserDetail
        /// <summary>
        /// UpdateUserDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>

        [HttpPut]
        public async Task<GavUser> UpdateUserDetail(GavUser userRequest)
        {
            var response = await _userRepository.UpdateUserDetail(userRequest);
            return response;
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
            var item = await _userRepository.GetUserByIdAndUserId(id, userId);
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
            bool isDeleted = await _userRepository.DeleteUser(id, userId);

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
