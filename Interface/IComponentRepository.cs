using CosmosDbDemo.Models;

namespace CosmosDbDemo.Interface
{
    public interface IComponentRepository:IRepository<ComponentEngagement>
    {

        Task<IEnumerable<ComponentEngagement>> GetByStatusAsync(string status);
        Task<IEnumerable<ComponentEngagement>> GetByCreatedByAsync(string email);
        Task<IEnumerable<ComponentEngagement>> GetByTeamUserAsync(string userEmail);
        Task<IEnumerable<ComponentEngagement>> GetByEngagementTypeAsync(string type);
        Task<IEnumerable<ComponentEngagement>> GetByOpinionIdAsync(string opinionId);
    }
}
