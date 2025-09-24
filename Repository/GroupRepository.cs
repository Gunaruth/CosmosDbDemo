using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosDbDemo.Repository
{
    public class GroupRepository : IGroupRepository
    {
        #region Declaration
        // private readonly ICosmosRepository _cosmosRepository;
        private readonly IRepository<AuditEngagement> _repository;
        private const string ContainerName = "groupContainer";
        private const string databaseName = "groupauditDB";
        private readonly CosmosClient _client;
        #endregion

        public GroupRepository(IRepository<AuditEngagement> repository, CosmosClient client)
        {
            // _cosmosRepository = cosmosRepository;
            _repository = repository;
            _client = client;
        }

        #region Add Group details
        /// <summary>
        /// AddGroupDetail
        /// </summary>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        public async Task<AuditEngagement> AddAsync(string containerName,AuditEngagement auditEngagement)
        {
            if (string.IsNullOrEmpty(auditEngagement.engagementId))
            {
                auditEngagement.engagementId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(auditEngagement.id))
            {
                auditEngagement.id = Guid.NewGuid().ToString();
            }
            return await _repository.AddAsync(containerName, auditEngagement);
        }
        #endregion

        #region Get All Group Details
        /// <summary>
        /// GetAllGroupDetails
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AuditEngagement>> GetAllAsync(string containerName)
        {
            return await _repository.GetAllAsync(containerName);
        }
        #endregion

        #region Update Group Detail
        /// <summary>
        /// UpdateGroupDetail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="auditEngagement"></param>
        /// <returns></returns>
        public async Task<AuditEngagement> UpdateAsync(string containerName, string id, AuditEngagement auditEngagement)
        {
            AuditEngagement response = new AuditEngagement();
            try
            {
                // Fetch the existing document by id and partition key
                var existingUser = await _repository.GetByIdAsync(containerName, auditEngagement.id, auditEngagement.engagementId);
                if (existingUser != null)
                {
                    existingUser.engagementType = auditEngagement.engagementType;
                    existingUser.location = auditEngagement.location;
                    existingUser.componentsLinked = auditEngagement.componentsLinked;
                    existingUser.status = auditEngagement.status;
                    existingUser.opinionId = auditEngagement.opinionId;
                    //existingUser.team = auditEngagement.team;
                    existingUser.isGroup = auditEngagement.isGroup;
                    response = await _repository.UpdateAsync(ContainerName, auditEngagement.engagementId, existingUser);
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

        public async Task<AuditEngagement> GetByIdAsync(string containerName,string id, string engagementId)
        {
            try
            {
                // Fetch the document by id and partition key
                return await _repository.GetByIdAsync(containerName, id, engagementId);
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
        public async Task<bool> DeleteAsync(string containerName, string id, string engagementId)
        {
            try
            {
                // Attempt to delete the document using id and partition key
                await _repository.DeleteAsync(containerName, id, engagementId);

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

        public async Task<IEnumerable<AuditEngagement>> GetByStatusAsync(string status)
        {
            var query = "SELECT * FROM c WHERE c.status = @status";
            var parameters = new Dictionary<string, object>
    {
        { "@status", status }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<AuditEngagement>> GetByCreatedByAsync(string email)
        {
            var query = "SELECT * FROM c WHERE c.createdBy = @createdBy";
            var parameters = new Dictionary<string, object>
    {
        { "@createdBy", email }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<AuditEngagement>> GetByComponentStatusAsync(string componentStatus)
        {
            var query = "SELECT * FROM c WHERE ARRAY_CONTAINS(c.componentsLinked, { \"status\": @status }, true)";
            var parameters = new Dictionary<string, object>
    {
        { "@status", componentStatus }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<AuditEngagement>> GetByTeamRoleAsync(string role)
        {
            var query = "SELECT * FROM c WHERE ARRAY_CONTAINS(c.team, { \"role\": @role }, true)";
            var parameters = new Dictionary<string, object>
    {
        { "@role", role }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<AuditEngagement>> GetByLocationAsync(double lat, double lng)
        {
            var query = "SELECT * FROM c WHERE c.location.lat = @lat AND c.location.lng = @lng";
            var parameters = new Dictionary<string, object>
    {
        { "@lat", lat },
        { "@lng", lng }
    };
            return await _repository.QueryAsync(ContainerName, query, parameters);
        }

        public async Task<IEnumerable<AuditEngagement>> QueryAsync(string containerName, string query, Dictionary<string, object> parameters)
        {
            var container = _client.GetContainer(databaseName, containerName);
            var queryDefinition = new QueryDefinition(query);

            foreach (var param in parameters)
            {
                queryDefinition.WithParameter(param.Key, param.Value);
            }

            var iterator = container.GetItemQueryIterator<AuditEngagement>(queryDefinition);
            var results = new List<AuditEngagement>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

    }


}
