using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class UserRepository : IUserRepository
    {
        #region Declaration

        private readonly Container _container;

        #endregion

        #region Constructor
        public UserRepository(CosmosClient cosmosClient, IConfiguration config)
        {
            var databaseName = config["CosmosDb:DatabaseName"];
            var containerName = config["CosmosDb:ContainerName"];

            EnsureDatabaseAndContainerExist(cosmosClient, databaseName, containerName).GetAwaiter().GetResult();
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }
        #endregion

        #region Add user details
        /// <summary>
        /// AdduserDetail
        /// </summary>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public async Task<GavUser> AdduserDetail(GavUser userRequest)
        {
            if (string.IsNullOrEmpty(userRequest.userId))
            {
                userRequest.userId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(userRequest.id))
            {
                userRequest.id = Guid.NewGuid().ToString();
            }
            ItemResponse<GavUser> response = await _container.CreateItemAsync(userRequest, new PartitionKey(userRequest.userId));
            return response.Resource;
        }
        #endregion

        #region Get User Details
        /// <summary>
        /// GetItemsAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<GavUser>> GetUsers()
        {
            var query = _container.GetItemQueryIterator<GavUser>("SELECT * FROM c");
            var results = new List<GavUser>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }
        #endregion

        #region Ensure Database And Container Exist
        /// <summary>
        /// EnsureDatabaseAndContainerExist
        /// </summary>
        /// <param name="cosmosClient"></param>
        /// <param name="databaseName"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private static async Task EnsureDatabaseAndContainerExist(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            // Create the database if it does not exist
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            Console.WriteLine($"Database {databaseName} created or already exists.");

            // Create the container if it does not exist, with partition key on 'userId'
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/userId");
            Console.WriteLine($"Container {containerName} created or already exists.");
        }
        #endregion

        #region Update UserDetail
        /// <summary>
        /// UpdateUserDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRequest"></param>
        /// <returns></returns>
        public async Task<GavUser> UpdateUserDetail(GavUser userRequest)
        {
            GavUser response = new GavUser();
            try
            {
                // Fetch the existing document by id and partition key
                ItemResponse<GavUser> userResponse = await _container.ReadItemAsync<GavUser>(userRequest.id, new PartitionKey(userRequest.userId));

                // If the document is found, we can update it
                var existingUser = userResponse.Resource;

                // Modify the properties of the existing user based on updatedUser
                if (existingUser != null)
                {
                    existingUser.userdetails = userRequest.userdetails;

                    if (existingUser?.engagement?.Count > 0 && userRequest.engagement.Count > 0)
                    {
                        var newEngagements = userRequest.engagement.Where(newEngagement => !existingUser.engagement
                                                .Any(existing => existing.engagementid == newEngagement.engagementid &&
                                                existing.type == newEngagement.type)).ToList();
                        if (newEngagements.Count > 0)
                        {
                            existingUser.engagement.AddRange(newEngagements);
                        }
                    }
                    else
                    {
                        existingUser.engagement = userRequest.engagement;
                    }
                    ItemResponse<GavUser> updatedResponse = await _container.ReplaceItemAsync(existingUser, userRequest.id, new PartitionKey(userRequest.userId));

                    // Return the updated document
                    response = updatedResponse.Resource;
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

        public async Task<GavUser> GetUserByIdAndUserId(string id, string userId)
        {
            try
            {
                // Fetch the document by id and partition key
                ItemResponse<GavUser> response = await _container.ReadItemAsync<GavUser>(id, new PartitionKey(userId));
                return response.Resource;
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
        public async Task<bool> DeleteUser(string id, string userId)
        {
            try
            {
                // Attempt to delete the document using id and partition key
                await _container.DeleteItemAsync<User>(id, new PartitionKey(userId));

                // If deletion succeeds, return true
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle case when the document is not found
                Console.WriteLine($"Document with id: {id} not found in partition: {userId}");
                return false;  // Return false if the document is not found
            }
            catch (Exception ex)
            {
                // Handle other errors
                Console.WriteLine($"Error occurred: {ex.Message}");
                return false;  // Return false in case of error
            }
        }
        #endregion
    }
}
