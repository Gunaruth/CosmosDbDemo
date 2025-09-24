using CosmosDbDemo.Models;

namespace CosmosDbDemo.Interface
{
    public interface ICommunicationRepository: IRepository<CommunicationDetails>
    {

        Task<IEnumerable<CommunicationDetails>> GetByStatusAsync(string status);
        Task<IEnumerable<CommunicationDetails>> GetByParticipantRoleAsync(string role);
        Task<IEnumerable<CommunicationDetails>> GetByParticipantUserIdAsync(string userId);
        Task<IEnumerable<CommunicationDetails>> GetByMessageSenderAsync(string senderId);
        Task<IEnumerable<CommunicationDetails>> GetByVisibleToUserAsync(string userId);

    }
}
