using CosmosDbDemo.Interface;
using CosmosDbDemo.Interfaces;
using CosmosDbDemo.Models;
using Microsoft.Azure.Cosmos;
using System.Net;
using static Azure.Core.HttpHeader;
namespace CosmosDbDemo.Repository
{
    public class CommunicationRepository : IRepository<CommunicationDetails>
    {
        #region Declaration

        private readonly Container _container;

        private readonly ICosmosRepository _cosmosRepository;
        private const string ContainerName = "Communication";

        #endregion

        #region Constructor
        public CommunicationRepository(ICosmosRepository cosmosRepository)
        {
            _cosmosRepository = cosmosRepository;
        }
        #endregion

        #region Add Chat Details
        /// <summary>
        /// AdduserDetail
        /// </summary>
        /// <param name="chatRequest"></param>
        /// <returns></returns>
        public async Task<CommunicationDetails> AddAsync(CommunicationDetails chatRequest)
        {
            if (string.IsNullOrEmpty(chatRequest.chatId))
            {
                chatRequest.chatId = Guid.NewGuid().ToString();
            }
            if (string.IsNullOrEmpty(chatRequest.id))
            {
                chatRequest.id = Guid.NewGuid().ToString();
            }
            return await _cosmosRepository.AddAsync(ContainerName, chatRequest);
        }
        #endregion
        #region Get Communication Details
        /// <summary>
        /// GetItemsAsync
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CommunicationDetails>> GetAllAsync()
        {
            return await _cosmosRepository.GetAllAsync<CommunicationDetails>(ContainerName);
        }
        #endregion
        #region Update ChatHistory
        /// <summary>
        /// UpdateUserDetail
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="newMessage"></param>
        /// <returns></returns>
        public async Task<CommunicationDetails> UpdateAsync(string chatId, CommunicationDetails comm)
        {
            CommunicationDetails response = null;
            try
            {
                // 1. Read existing chat document by chatId and partitionKey (usually chatId or something else)
                var chatResponse =
                    await GetByIdAsync(comm.id, chatId);

                var existingChat = chatResponse;

                if (existingChat != null)
                {
                    // 2. Initialize Messages list if null
                    if (existingChat.Messages == null)
                    {
                        existingChat.Messages = new List<Message>();
                    }

                    // 3. Add the new message to Messages
                    existingChat.Messages.AddRange(comm.Messages);

                    // 4. Update the chat document in Cosmos DB (replace)
                    response = await _cosmosRepository.UpdateAsync(ContainerName, existingChat.chatId, existingChat);

                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Chat document with id: {chatId} not found in partition: {chatId}");
                return null;
            }

            return response;
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// DeleteUser
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string chatId, string userId)
        {
            try
            {
                // Attempt to delete the document using id and partition key
                await _cosmosRepository.DeleteAsync<CommunicationDetails>(ContainerName, chatId, userId);

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
          #region Get UserById And UserId
        /// <summary>
        /// GetUserByIdAndUserIdAsync
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        public async Task<CommunicationDetails> GetByIdAsync(string id, string chatid)
        {
            try
            {
                // Fetch the document by id and partition key
                return await _cosmosRepository.GetByIdAsync<CommunicationDetails>(ContainerName, id, chatid);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Handle the case when the document is not found
                Console.WriteLine($"Document with id: {id} not found in partition: {chatid}");
                return null;  // Return null if the document is not found
            }
        }
        #endregion
    }
}
