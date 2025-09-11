using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class ComponentRepository : IRepository<AuditEngagement>
    {
        #region Declaration
        private readonly ICosmosRepository _cosmosRepository;
        private const string ContainerName = "componentContainer";

        #endregion

        public ComponentRepository(ICosmosRepository cosmosRepository)
        {
            _cosmosRepository = cosmosRepository;
        }

        #region Add Component details
        /// <summary>
        /// Add Component
        /// </summary>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        public async Task<AuditEngagement> AddAsync(AuditEngagement auditEngagement)
        {
            if (string.IsNullOrEmpty(auditEngagement.userId))
            {
                auditEngagement.userId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(auditEngagement.id))
            {
                auditEngagement.id = Guid.NewGuid().ToString();
            }
            return await _cosmosRepository.AddAsync(ContainerName, auditEngagement);
        }
        #endregion

        #region Get All Component Details
        /// <summary>
        /// GetAllAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AuditEngagement>> GetAllAsync()
        {
            return await _cosmosRepository.GetAllAsync<AuditEngagement>(ContainerName);
        }
        #endregion

        #region Update Component Detail
        /// <summary>
        /// UpdateComponentDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        public async Task<AuditEngagement> UpdateAsync(string id, AuditEngagement auditEngagement)
        {
            AuditEngagement response = new AuditEngagement();
            try
            {
                // Fetch the existing document by id and partition key
                var existingUser = await _cosmosRepository.GetByIdAsync<AuditEngagement>(ContainerName, auditEngagement.id, auditEngagement.userId);

                // If the document is found, we can update it
                //var existingUser = userResponse.Resource;

                // Modify the properties of the existing user based on updatedUser
                //if (existingUser != null)
                //{
                //    existingUser.userdetails = userRequest.userdetails;

                //    if (existingUser?.engagement?.Count > 0 && userRequest.engagement.Count > 0)
                //    {
                //        var newEngagements = userRequest.engagement.Where(newEngagement => !existingUser.engagement
                //                                .Any(existing => existing.engagementid == newEngagement.engagementid &&
                //                                existing.type == newEngagement.type)).ToList();
                //        if (newEngagements.Count > 0)
                //        {
                //            existingUser.engagement.AddRange(newEngagements);
                //        }
                //    }
                //    else
                //    {
                //        existingUser.engagement = userRequest.engagement;
                //    }
                //    response = await _cosmosRepository.UpdateAsync(ContainerName, userRequest.userId, existingUser);
                //    //  await _container.ReplaceItemAsync(existingUser, userRequest.id, new PartitionKey(userRequest.userId));
                //}
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle the case when the document is not found
                Console.WriteLine($"Document with id: {auditEngagement.id} not found in partition: {auditEngagement.userId}");
                return null;  // Return null if document is not found
            }
            return response;
        }
        #endregion

        #region Get Component By EngagementId 
        /// <summary>
        /// GetComponentByEngagementId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engagementId"></param>
        /// <returns></returns>

        public async Task<AuditEngagement> GetByIdAsync(string id, string engagementId)
        {
            try
            {
                // Fetch the document by id and partition key
                return await _cosmosRepository.GetByIdAsync<AuditEngagement>(ContainerName, id, engagementId);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle the case when the document is not found
                Console.WriteLine($"Document with id: {id} not found in partition: {engagementId}");
                return null;  // Return null if the document is not found
            }
        }
        #endregion

        #region DeleteComponent
        /// <summary>
        /// DeleteComponent
        /// </summary>
        /// <param name="id"></param>
        /// <param name="engagementId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string id, string engagementId)
        {
            try
            {
                // Attempt to delete the document using id and partition key
                await _cosmosRepository.DeleteAsync<AuditEngagement>(ContainerName, id, engagementId);

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
