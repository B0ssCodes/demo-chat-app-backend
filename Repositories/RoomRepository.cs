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

        // Get the SignalR hub with dependency injection
        private readonly IHubContext<ChatHub> _chatHub;

        public RoomRepository(ApplicationDbContext db, IHubContext<ChatHub> chatHub)
        {
            _db = db;
            _chatHub = chatHub;
        }

        
        public async Task<RoomResponseDTO> AddUserToRoom(int userId, int roomId)
        {
            // Find the user
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Find the room and include users and messages to send back in the response
            var room = await _db.Rooms
                .Include(r => r.Users) 
                .Include(r => r.Messages) 
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                throw new Exception("Room not found");
            }

            
            if (room.Users.Any(u => u.UserId == userId))
            {
                throw new Exception("User is already in the room");
            }

            // Add the user to the room and save
            room.Users.Add(user);

            await _db.SaveChangesAsync();

            // return a RoomResponseDTO with the newly added user
            RoomResponseDTO response = new RoomResponseDTO
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                UserCount = room.Users.Count,
                Users = room.Users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Description = u.Description,
                }).ToList(),

            };

            // Send a message to the room that a user has been added
            await _chatHub.Clients.Group(roomId.ToString()).SendAsync("UserAddedToRoom", response);
            return response;
        }

        public async Task<RoomResponseDTO> CreateRoom(RoomRequestDTO roomDTO)
        {
            // Find the user that is creating the room
            User user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == roomDTO.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            // Create a new room with the user as the first member
            Room room = new Room
            {
                Name = roomDTO.Name,
                Description = roomDTO.Description,
                Users = new List<User>() { user },
                Messages = new List<Message>(),
            };
            // Add the room to the database and save
            await _db.Rooms.AddAsync(room);
            await _db.SaveChangesAsync();

          
            // Map the room to a RoomResponseDTO
            RoomResponseDTO response = new()
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                UserCount = room.Users.Count,
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
            // Get the room with all the properties from DB
            Room room = await _db.Rooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                throw new Exception("Room not found");
            }

            // Map the room to a RoomResponseDTO
            RoomResponseDTO response = new()
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                UserCount = room.Users.Count,
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
            // Get all the rooms with all the properties from DB
            var rooms = await _db.Rooms
                .Include(r => r.Users)
                // Include messages to count them
                .Include(r => r.Messages)
                .ToListAsync();

            if (rooms == null)
            {
                throw new Exception("Rooms not found");
            }

            // Map the rooms to roomResponseDTOs
            var roomDTOs = rooms.Select(room => new RoomResponseDTO
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                UserCount = room.Users.Count,
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
            // Get the rooms based on the userId
            var rooms = await _db.Rooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .Where(r => r.Users.Any(u => u.UserId == userId))
                .ToListAsync();

            if (rooms == null)
            {
                throw new Exception("Rooms not found, or User ID invalid");
            }

            // Map the rooms to roomResponseDTOs
            var roomDTOs = rooms.Select(room => new RoomResponseDTO
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Description = room.Description,
                MessageCount = room.Messages.Count,
                UserCount = room.Users.Count,
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

        public async Task RemoveUserFromRoom(int userId, int roomId)
        {
            // Check if the user exists
            User user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Find the room
            var room = await _db.Rooms
                .Include(r => r.Users)
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                throw new Exception("Room not found");
            }

            // Check if the user is in the room
            if (!room.Users.Any(u => u.UserId == userId))
            {
                throw new Exception("User is not in the room");
            }

            // Remove the user from the room
            room.Users.Remove(user);

            // if the room is empty, delete it
            if (room.Users.Count == 0)
            {
               _db.Rooms.Remove(room);
            }

            await _db.SaveChangesAsync();
        }
    }
}
