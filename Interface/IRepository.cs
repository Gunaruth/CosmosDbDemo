namespace CosmosDbDemo.Interface
{
    public interface IRepository<T> where T : class
    {

        Task<IEnumerable<T>> GetAllAsync(string containerName);
        Task<T> GetByIdAsync(string containerName, string id, string userId);
        Task<T> AddAsync(string containerName, T entity);
        Task<T> UpdateAsync(string containerName, string id, T entity);
        Task<bool> DeleteAsync(string containerName, string id, string userId);
        Task<IEnumerable<T>> QueryAsync(string containerName, string query, Dictionary<string, object> parameters);
    }
}
