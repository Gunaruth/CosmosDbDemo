using CosmosDbDemo.Interface;
using CosmosDbDemo.Models;
using System.Reflection;

namespace CosmosDbDemo.Interfaces
{
    public interface IUserRepository : IRepository<GavUser>
    {

        Task<IEnumerable<GavUser>> GetUsersByUsernameAsync(string username);
        Task<IEnumerable<GavUser>> GetUsersByRegionAsync(string region);
        Task<IEnumerable<GavUser>> GetUsersByEngagementRoleAsync(string role);
        Task<IEnumerable<GavUser>> GetUsersByEngagementStatusAsync(string status);

    }
}
