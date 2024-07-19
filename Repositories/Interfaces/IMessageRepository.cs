using ChatApp.Models.Dto;

namespace ChatApp.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        // Returns a collection of all the room's messages
        Task<List<MessageResponseDTO>> GetRoomMessages(int roomId);
        //Returns a collection of all the user's messages
        Task<List<MessageResponseDTO>> GetUserMessages(int userId);
        // Adds a new message to the database and returns it.
        Task<MessageResponseDTO> CreateMessage(MessageRequestDTO messageDTO);
    }
}
