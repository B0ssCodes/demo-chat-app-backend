using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<ChatHub> _chatHub;

        public RoomRepository(ApplicationDbContext db, IHubContext<ChatHub> chatHub)
        {
            _db = db;
            _chatHub = chatHub;
        }

        public async Task AddUserToRoom(int userId, int roomId)
        {
            // Find the user
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Find the room
            var room = await _db.Rooms
                .Include(r => r.Users) // Include existing users to avoid overwriting them
                .Include(r => r.Messages) // Include messages if you want them in the response
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room == null)
            {
                throw new Exception("Room not found");
            }

            // Add the user to the room
            room.Users.Add(user);

            // Save changes to the database
            await _db.SaveChangesAsync();

            // Map the updated room to RoomResponseDTO
            RoomResponseDTO response = new RoomResponseDTO
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                Users = room.Users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Description = u.Description,
                }).ToList(),
                
            };

            await _chatHub.Clients.Group(roomId.ToString()).SendAsync("UserAddedToRoom", response);
        }

        public async Task<RoomResponseDTO> CreateRoom(RoomRequestDTO roomDTO)
        {
            User user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == roomDTO.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            Room room = new Room
            {
                Name = roomDTO.Name,
                Description = roomDTO.Description,
                Users = new List<User>() { user },
                Messages = new List<Message>(),
            };

            await _db.Rooms.AddAsync(room);
            await _db.SaveChangesAsync();

          
            // Map Users and Messages to their respective DTOs
            RoomResponseDTO response = new()
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                Users = room.Users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Description = u.Description,
                  
                }).ToList(),
            };

            return response;
        }


        public async Task<RoomResponseDTO> GetRoom(int roomId)
        {
            Room room = await _db.Rooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                throw new Exception("Room not found");
            }

            RoomResponseDTO response = new()
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                Users = room.Users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Description = u.Description,

                }).ToList(),

            };

            return response;
        }

        public async Task<List<RoomResponseDTO>> GetRooms()
        {
            // Fetch rooms with their users and messages to count them
            var rooms = await _db.Rooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .ToListAsync();

            if (rooms == null)
            {
                throw new Exception("Rooms not found");
            }

            // Project each Room to a RoomResponseDTO, including the message count
            var roomDTOs = rooms.Select(room => new RoomResponseDTO
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                Users = room.Users.Select(user => new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Description = user.Description,
                }).ToList(),
             
            });

            return roomDTOs.ToList();
        }

        public async Task<List<RoomResponseDTO>> GetRoomsByUser(int userId)
        {
            var rooms = await _db.Rooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .Where(r => r.Users.Any(u => u.UserId == userId))
                .ToListAsync();

            if (rooms == null)
            {
                throw new Exception("Rooms not found, or User ID invalid");
            }
            var roomDTOs = rooms.Select(room => new RoomResponseDTO
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                Users = room.Users.Select(user => new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Description = user.Description,
                }).ToList(),
                
            });

            return roomDTOs.ToList();
        }
    }
}
