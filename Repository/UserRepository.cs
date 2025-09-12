using CosmosDbDemo.Interface;
using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class UserRepository : IRepository<GavUser>
    {
        #region Declaration

        private readonly ICosmosRepository _cosmosRepository;
        private const string ContainerName = "usersContainer";

        #endregion

        #region Constructor
        public UserRepository(ICosmosRepository cosmosRepository)
        {
            _cosmosRepository = cosmosRepository;
        }
        #endregion

        #region Add user details
        /// <summary>
        /// AdduserDetail
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public async Task<GavUser> AddAsync(GavUser userRequest)
        {
            if (string.IsNullOrEmpty(userRequest.userId))
            {
                userRequest.userId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(userRequest.id))
            {
                userRequest.id = Guid.NewGuid().ToString();
            }
            return await _cosmosRepository.AddAsync(ContainerName, userRequest);
        }
        #endregion

        #region Get User Details
        /// <summary>
        /// GetItemsAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<GavUser>> GetAllAsync()
        {
            return await _cosmosRepository.GetAllAsync<GavUser>(ContainerName);
        }
        #endregion

        #region Update UserDetail
        /// <summary>
        /// UpdateUserDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public async Task<GavUser> UpdateAsync(string id, GavUser userRequest)
        {
            GavUser response = new GavUser();
            try
            {
                // Fetch the existing document by id and partition key
                var existingUser = await GetByIdAsync(userRequest.id, userRequest.userId);

                // If the document is found, we can update it
                //var existingUser = userResponse.Resource;

                // Modify the properties of the existing user based on updatedUser
                if (existingUser != null)
                {
                    existingUser.userdetails = userRequest.userdetails;
                    existingUser.engagement = userRequest.engagement;
                    
                    response = await _cosmosRepository.UpdateAsync(ContainerName, userRequest.userId, existingUser);
                    //  await _container.ReplaceItemAsync(existingUser, userRequest.id, new PartitionKey(userRequest.userId));
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle the case when the document is not found
                Console.WriteLine($"Document with id: {userRequest.id} not found in partition: {userRequest.userId}");
                return null;  // Return null if document is not found
            }
            return response;
        }
        #endregion

        #region Get UserById And UserId
        /// <summary>
        /// GetUserByIdAndUserIdAsync
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        public async Task<GavUser> GetByIdAsync(string id, string userId)
        {
            try
            {
                // Fetch the document by id and partition key
                return await _cosmosRepository.GetByIdAsync<GavUser>(ContainerName, id, userId);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle the case when the document is not found
                Console.WriteLine($"Document with id: {id} not found in partition: {userId}");
                return null;  // Return null if the document is not found
            }
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// DeleteUser
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string id, string userId)
        {
            try
            {
                // Attempt to delete the document using id and partition key
                await _cosmosRepository.DeleteAsync<GavUser>(ContainerName, id, userId);

                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;  // Return false if the document is not found
            }
            catch (Exception ex)
            {
                return false;  // Return false in case of error
            }
        }
        #endregion
    }
}
