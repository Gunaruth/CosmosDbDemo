using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace CosmosDbDemo.Repository
{
    public class CosmosRepository<T> : IRepository<T> where T : class
    {
        private readonly CosmosClient _client;
        private readonly string _databaseName;

        public CosmosRepository(CosmosClient client, IOptions<CosmosDbSettings> settings)
        {
            _client = client;
            _databaseName = settings.Value.DatabaseName;
        }

        private Container GetContainer(string containerName)
        {
            return _client.GetContainer(_databaseName, containerName);
        }

        public async Task<IEnumerable<T>> GetAllAsync(string containerName)
        {
            var container = GetContainer(containerName);
            var query = container.GetItemQueryIterator<T>("SELECT * FROM c");
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<T> GetByIdAsync(string containerName, string id,string userId)
        {
            var container = GetContainer(containerName);
            try
            {
                var response = await container.ReadItemAsync<T>(id, new PartitionKey(userId));
                return response.Resource;
            }
            catch (CosmosException)
            {
                return default;
            }
        }

        public async Task<T> AddAsync(string containerName, T entity)
        {
            var container = GetContainer(containerName);
            var response = await container.CreateItemAsync(entity, new PartitionKey(GetId(entity)));
            return response.Resource;
        }

        public async Task<T> UpdateAsync(string containerName, string id, T entity)
        {
            var container = GetContainer(containerName);
            var response = await container.UpsertItemAsync(entity, new PartitionKey(id));
            return response.Resource;
        }

        public async Task<bool> DeleteAsync(string containerName, string id,string userId)
        {

            var container = GetContainer(containerName);
            try
            {
                await container.DeleteItemAsync<T>(id, new PartitionKey(userId));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            catch
            {
                return false;
            }

        }
        public async Task<IEnumerable<T>> QueryAsync(string containerName, string query, Dictionary<string, object> parameters)
        {
            var container = GetContainer(containerName);
            var queryDefinition = new QueryDefinition(query);

            foreach (var param in parameters)
            {
                queryDefinition.WithParameter(param.Key, param.Value);
            }

            var iterator = container.GetItemQueryIterator<T>(queryDefinition);
            var results = new List<T>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        private string GetId(T entity)
        {
            var prop = typeof(T).GetProperty("userId") ?? typeof(T).GetProperty("engagementId") ?? typeof(T).GetProperty("chatId");
            return prop?.GetValue(entity)?.ToString();
        }
    }

}
