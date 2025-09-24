using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class ComponentRepository : IComponentRepository
    {
        #region Declaration
        //private readonly ICosmosRepository _cosmosRepository;
        private const string ContainerName = "componentContainer";
        private readonly IRepository<ComponentEngagement> _repository;
        private readonly CosmosClient _client;
        private const string databaseName = "groupauditDB";
        #endregion

        public ComponentRepository(IRepository<ComponentEngagement> repository, CosmosClient client)
        {
            //_cosmosRepository = cosmosRepository;
            _repository = repository;
            _client = client;
        }

        #region Add Component details
        /// <summary>
        /// Add Component
        /// </summary>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        public async Task<ComponentEngagement> AddAsync(string containerName, ComponentEngagement auditEngagement)
        {
            if (string.IsNullOrEmpty(auditEngagement.engagementId))
            {
                auditEngagement.engagementId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(auditEngagement.id))
            {
                auditEngagement.id = Guid.NewGuid().ToString();
            }
            return await _repository.AddAsync(ContainerName, auditEngagement);
        }
        #endregion

        #region Get All Component Details
        /// <summary>
        /// GetAllAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ComponentEngagement>> GetAllAsync(string containerName)
        {
            return await _repository.GetAllAsync(ContainerName);
        }
        #endregion

        #region Update Component Detail
        /// <summary>
        /// UpdateComponentDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="compEngagement"></param>
        /// <returns></returns>
        public async Task<ComponentEngagement> UpdateAsync(string containerName, string id, ComponentEngagement compEngagement)
        {
            ComponentEngagement response = new ComponentEngagement();
            try
            {
                // Fetch the existing document by id and partition key
                var existingComponent = await GetByIdAsync(containerName,compEngagement.id, compEngagement.engagementId);
                if (existingComponent != null)
                {
                    existingComponent.engagementType = compEngagement.engagementType;
                    existingComponent.location = compEngagement.location;
                    existingComponent.groupLinked = compEngagement.groupLinked;
                    existingComponent.status = compEngagement.status;
                    existingComponent.opinionId = compEngagement.opinionId;
                    existingComponent.opinionOptions = compEngagement.opinionOptions;

                    //existingComponent.team = compEngagement.team;
                    response = await _repository.UpdateAsync(ContainerName, compEngagement.engagementId, existingComponent);
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

        public async Task<ComponentEngagement> GetByIdAsync(string containerName, string id, string engagementId)
        {
            try
            {
                // Fetch the document by id and partition key
                return await _repository.GetByIdAsync(ContainerName, id, engagementId);
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
        public async Task<bool> DeleteAsync(string containerName, string id, string engagementId)
        {
            try
            {
                // Attempt to delete the document using id and partition key
                await _repository.DeleteAsync(ContainerName, id, engagementId);

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

        public async Task<IEnumerable<ComponentEngagement>> GetByStatusAsync(string status)
        {
            var query = "SELECT * FROM c WHERE c.status = @status";
            var parameters = new Dictionary<string, object> { { "@status", status } };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<ComponentEngagement>> GetByCreatedByAsync(string email)
        {
            var query = "SELECT * FROM c WHERE c.createdBy = @createdBy";
            var parameters = new Dictionary<string, object> { { "@createdBy", email } };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<ComponentEngagement>> GetByTeamUserAsync(string userEmail)
        {
            var query = "SELECT * FROM c WHERE ARRAY_CONTAINS(c.team, { \"user\": @user }, true)";
            var parameters = new Dictionary<string, object> { { "@user", userEmail } };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<ComponentEngagement>> GetByEngagementTypeAsync(string type)
        {
            var query = "SELECT * FROM c WHERE c.engagementType = @type";
            var parameters = new Dictionary<string, object> { { "@type", type } };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<ComponentEngagement>> GetByOpinionIdAsync(string opinionId)
        {
            var query = "SELECT * FROM c WHERE c.opinionId = @opinionId";
            var parameters = new Dictionary<string, object> { { "@opinionId", opinionId } };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }
        public async Task<IEnumerable<ComponentEngagement>> QueryAsync(string containerName, string query, Dictionary<string, object> parameters)
        {
            var container = _client.GetContainer(databaseName, containerName);
            var queryDefinition = new QueryDefinition(query);

            foreach (var param in parameters)
            {
                queryDefinition.WithParameter(param.Key, param.Value);
            }

            var iterator = container.GetItemQueryIterator<ComponentEngagement>(queryDefinition);
            var results = new List<ComponentEngagement>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }
    }
}
