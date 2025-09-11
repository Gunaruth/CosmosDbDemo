namespace CosmosDbDemo.Interface
{
    public interface ICosmosRepository
    {
        Task<IEnumerable<T>> GetAllAsync<T>(string containerName);
        Task<T> GetByIdAsync<T>(string containerName, string id, string userId);
        Task<T> AddAsync<T>(string containerName, T entity);
        Task<T> UpdateAsync<T>(string containerName, string id, T entity);
        Task<bool> DeleteAsync<T>(string containerName, string id, string userId);
    }

}
