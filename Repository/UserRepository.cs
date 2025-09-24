using CosmosDbDemo.Interface;
using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class UserRepository : IUserRepository
    {
        #region Declaration

        //private readonly ICosmosRepository _cosmosRepository;
        private const string ContainerName = "usersContainer";
        private readonly IRepository<GavUser> _repository;
        private readonly CosmosClient _client;
        private const string databaseName = "groupauditDB";
        #endregion

        #region Constructor
        public UserRepository(IRepository<GavUser> repository, CosmosClient client)
        {
            //_cosmosRepository = cosmosRepository;
            _repository = repository;
            _client= client;
        }
        #endregion

        #region Add user details
        /// <summary>
        /// AdduserDetail
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public async Task<GavUser> AddAsync(string containerName, GavUser userRequest)
        {
            if (string.IsNullOrEmpty(userRequest.userId))
            {
                userRequest.userId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(userRequest.id))
            {
                userRequest.id = Guid.NewGuid().ToString();
            }
            return await _repository.AddAsync(containerName, userRequest);
        }
        #endregion

        #region Get User Details
        /// <summary>
        /// GetItemsAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<GavUser>> GetAllAsync(string containerName)
        {
            return await _repository.GetAllAsync(containerName);
        }
        #endregion

        #region Update UserDetail
        /// <summary>
        /// UpdateUserDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public async Task<GavUser> UpdateAsync(string containerName,string id, GavUser userRequest)
        {
            GavUser response = new GavUser();
            try
            {
                // Fetch the existing document by id and partition key
                var existingUser = await GetByIdAsync(containerName,userRequest.id, userRequest.userId);

                // If the document is found, we can update it
                //var existingUser = userResponse.Resource;

                // Modify the properties of the existing user based on updatedUser
                if (existingUser != null)
                {
                    existingUser.userdetails = userRequest.userdetails;
                    existingUser.engagement = userRequest.engagement;
                    
                    response = await _repository.UpdateAsync(containerName, userRequest.userId, existingUser);
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

        public async Task<GavUser> GetByIdAsync(string containerName,string id, string userId)
        {
            try
            {
                // Fetch the document by id and partition key
                return await _repository.GetByIdAsync(ContainerName, id, userId);
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
        public async Task<bool> DeleteAsync(string containerName, string id, string userId)
        {
            try
            {
                // Attempt to delete the document using id and partition key
                await _repository.DeleteAsync(ContainerName, id, userId);

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

        public async Task<IEnumerable<GavUser>> GetUsersByUsernameAsync(string username)
        {
            var query = "SELECT * FROM c WHERE c.username = @username";
            var parameters = new Dictionary<string, object>
    {
        { "@username", username }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<GavUser>> GetUsersByRegionAsync(string region)
        {
            var query = "SELECT * FROM c WHERE c.userdetails.region = @region";
            var parameters = new Dictionary<string, object>
    {
        { "@region", region }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<GavUser>> GetUsersByEngagementRoleAsync(string role)
        {
            var query = "SELECT * FROM c WHERE ARRAY_CONTAINS(c.engagement, { \"role\": @role }, true)";
            var parameters = new Dictionary<string, object>
    {
        { "@role", role }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<GavUser>> GetUsersByEngagementStatusAsync(string status)
        {
            var query = "SELECT * FROM c WHERE ARRAY_CONTAINS(c.engagement, { \"status\": @status }, true)";
            var parameters = new Dictionary<string, object>
    {
        { "@status", status }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<GavUser>> QueryAsync(string containerName, string query, Dictionary<string, object> parameters)
        {
            var container = _client.GetContainer(databaseName, containerName);
            var queryDefinition = new QueryDefinition(query);

            foreach (var param in parameters)
            {
                queryDefinition.WithParameter(param.Key, param.Value);
            }

            var iterator = container.GetItemQueryIterator<GavUser>(queryDefinition);
            var results = new List<GavUser>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

    }
}
