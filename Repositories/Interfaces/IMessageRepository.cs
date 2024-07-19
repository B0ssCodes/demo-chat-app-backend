using ChatApp.Models.Dto;

namespace ChatApp.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        // Returns a collection of all the room's messages
        Task<ICollection<MessageResponseDTO>> GetRoomMessages(int roomId);
        //Returns a collection of all the user's messages
        Task<ICollection<MessageResponseDTO>> GetUserMessages(int userId);
        // Adds a new message to the database and returns it.
        Task<MessageResponseDTO> CreateMessage(MessageRequestDTO messageDTO);
    }
}
