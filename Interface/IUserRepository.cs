using System.Reflection;

namespace CosmosDbDemo.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<Models.GavUser>> GetUsers();
        Task<Models.GavUser> AdduserDetail(Models.GavUser userRequest);
        Task<Models.GavUser> UpdateUserDetail(Models.GavUser userRequest);
        Task<Models.GavUser> GetUserByIdAndUserId(string id, string userId);
        Task<bool> DeleteUser(string id, string userId);
    }
}
