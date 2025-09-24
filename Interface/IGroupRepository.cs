using CosmosDbDemo.Models;

namespace CosmosDbDemo.Interface
{
    public interface IGroupRepository:IRepository<AuditEngagement>
    {

        Task<IEnumerable<AuditEngagement>> GetByStatusAsync(string status);
        Task<IEnumerable<AuditEngagement>> GetByCreatedByAsync(string email);
        Task<IEnumerable<AuditEngagement>> GetByComponentStatusAsync(string componentStatus);
        Task<IEnumerable<AuditEngagement>> GetByTeamRoleAsync(string role);
        Task<IEnumerable<AuditEngagement>> GetByLocationAsync(double lat, double lng);

    }
}
