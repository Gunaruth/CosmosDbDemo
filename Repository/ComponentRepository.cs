using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class ComponentRepository : IRepository<ComponentEngagement>
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
        public async Task<ComponentEngagement> AddAsync(ComponentEngagement auditEngagement)
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

        #region Get All Component Details
        /// <summary>
        /// GetAllAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ComponentEngagement>> GetAllAsync()
        {
            return await _cosmosRepository.GetAllAsync<ComponentEngagement>(ContainerName);
        }
        #endregion

        #region Update Component Detail
        /// <summary>
        /// UpdateComponentDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="compEngagement"></param>
        /// <returns></returns>
        public async Task<ComponentEngagement> UpdateAsync(string id, ComponentEngagement compEngagement)
        {
            ComponentEngagement response = new ComponentEngagement();
            try
            {
                // Fetch the existing document by id and partition key
                var existingComponent = await GetByIdAsync(compEngagement.id, compEngagement.engagementId);
                if (existingComponent != null)
                {
                    existingComponent.engagementType = compEngagement.engagementType;
                    existingComponent.location = compEngagement.location;
                    existingComponent.groupLinked = compEngagement.groupLinked;
                    existingComponent.status = compEngagement.status;
                    existingComponent.opinionId = compEngagement.opinionId;
                    existingComponent.opinionOptions = compEngagement.opinionOptions;
                    existingComponent.team = compEngagement.team;
                    response = await _cosmosRepository.UpdateAsync(ContainerName, compEngagement.engagementId, existingComponent);
                }
                
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle the case when the document is not found
                Console.WriteLine($"Document with id: {compEngagement.id} not found in partition: {compEngagement.engagementId}");
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

        public async Task<ComponentEngagement> GetByIdAsync(string id, string engagementId)
        {
            try
            {
                // Fetch the document by id and partition key
                return await _cosmosRepository.GetByIdAsync<ComponentEngagement>(ContainerName, id, engagementId);
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
                await _cosmosRepository.DeleteAsync<ComponentEngagement>(ContainerName, id, engagementId);

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
