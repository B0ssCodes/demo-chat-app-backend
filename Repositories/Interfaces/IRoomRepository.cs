using ChatApp.Models.Dto;

namespace ChatApp.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        // Returns a collection of all the rooms
        Task<List<RoomResponseDTO>> GetRooms();
        // Returns a collection of all the rooms that a user is in
        Task<List<RoomResponseDTO>> GetRoomsByUser(int userId);
        // Returns a room by its id
        Task<RoomResponseDTO> GetRoom(int roomId);
        // Adds a new room to the database and returns it.
        Task<RoomResponseDTO> CreateRoom(RoomRequestDTO roomDTO);
        Task<RoomResponseDTO> AddUserToRoom(int userId, int roomId);
        Task RemoveUserFromRoom(int userId, int roomId);
    }
}
