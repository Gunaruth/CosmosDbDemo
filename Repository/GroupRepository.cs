using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class GroupRepository : IRepository<AuditEngagement>
    {
        #region Declaration
        private readonly ICosmosRepository _cosmosRepository;
        private const string ContainerName = "groupContainer";

        #endregion

        public GroupRepository(ICosmosRepository cosmosRepository)
        {
            _cosmosRepository = cosmosRepository;
        }

        #region Add Group details
        /// <summary>
        /// AddGroupDetail
        /// </summary>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        public async Task<AuditEngagement> AddAsync(AuditEngagement auditEngagement)
        {
            if (string.IsNullOrEmpty(auditEngagement.engagementId))
            {
                auditEngagement.engagementId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(auditEngagement.id))
            {
                auditEngagement.id = Guid.NewGuid().ToString();
            }
            return await _cosmosRepository.AddAsync(ContainerName, auditEngagement);
        }
        #endregion

        #region Get All Group Details
        /// <summary>
        /// GetAllGroupDetails
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AuditEngagement>> GetAllAsync()
        {
            return await _cosmosRepository.GetAllAsync<AuditEngagement>(ContainerName);
        }
        #endregion

        #region Update Group Detail
        /// <summary>
        /// UpdateGroupDetail
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
                var existingUser = await _cosmosRepository.GetByIdAsync<AuditEngagement>(ContainerName, auditEngagement.id, auditEngagement.engagementId);
                if (existingUser != null)
                {
                    existingUser.engagementType = auditEngagement.engagementType;
                    existingUser.location = auditEngagement.location;
                    existingUser.componentsLinked = auditEngagement.componentsLinked;
                    existingUser.status = auditEngagement.status;
                    existingUser.opinionId = auditEngagement.opinionId;
                    existingUser.team = auditEngagement.team;
                    existingUser.isGroup = auditEngagement.isGroup;
                    response = await _cosmosRepository.UpdateAsync(ContainerName, auditEngagement.engagementId, existingUser);
                }
                else
                {
                    return null;
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle the case when the document is not found
                Console.WriteLine($"Document with id: {auditEngagement.id} not found in partition: {auditEngagement.engagementId}");
                return null;  // Return null if document is not found
            }
            return response;
        }
        #endregion

        #region Get GroupDetail By EngagementId
        /// <summary>
        /// GetGroupDetailByEngagementId
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

        #region DeleteGroup
        /// <summary>
        /// DeleteGroup
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
