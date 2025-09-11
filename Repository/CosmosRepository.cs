using CosmosDbDemo.Interface;
using Microsoft.Azure.Cosmos;

namespace CosmosDbDemo.Repository
{
    public class CosmosRepository : ICosmosRepository
    {
        private readonly CosmosClient _client;
        private readonly string _databaseName;

        public CosmosRepository(CosmosClient client, string databaseName)
        {
            _client = client;
            _databaseName = databaseName;
        }

        private Container GetContainer(string containerName)
        {
            return _client.GetContainer(_databaseName, containerName);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string containerName)
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

        public async Task<T> GetByIdAsync<T>(string containerName, string id,string userId)
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

        public async Task<T> AddAsync<T>(string containerName, T entity)
        {
            var container = GetContainer(containerName);
            var response = await container.CreateItemAsync(entity, new PartitionKey(GetId(entity)));
            return response.Resource;
        }

        public async Task<T> UpdateAsync<T>(string containerName, string id, T entity)
        {
            var container = GetContainer(containerName);
            var response = await container.UpsertItemAsync(entity, new PartitionKey(id));
            return response.Resource;
        }

        public async Task<bool> DeleteAsync<T>(string containerName, string id,string userId)
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

        private string GetId<T>(T entity)
        {
            var prop = typeof(T).GetProperty("Id");
            return prop?.GetValue(entity)?.ToString();
        }
    }

}
