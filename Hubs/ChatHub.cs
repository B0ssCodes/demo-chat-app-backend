using ChatApp.Data;
using ChatApp.DataService;
using ChatApp.Models;
using ChatApp.Models.Dto;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    // Extend the base Hub class
    public class ChatHub : Hub
    {
        // Using dependancy injection to get the MemoryDb instance
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
          
            var userIdString = httpContext.Request.Query["userId"];
            var roomIdString = httpContext.Request.Query["roomId"];

            if (!int.TryParse(userIdString, out int userId) || !int.TryParse(roomIdString, out int roomId))
            {
                throw new Exception("Invalid user ID or room ID");
            }

            // Optionally, verify that the user is a member of the specified room
            var isUserInRoom = _db.Rooms.Any(r => r.RoomId == roomId && r.Users.Any(u => u.UserId == userId));
            if (!isUserInRoom)
            {
                throw new Exception("User is not a member of the specified room");
            }

            // Connect the user to the specified room's SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

            await base.OnConnectedAsync();
        }


        public async Task SendMessage(int userId, int roomId, string messageContent)
        {
            try
            {

                User user = await _db.Users.FindAsync(userId);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                Room room = await _db.Rooms
                                      .Include(r => r.Users)
                                      .FirstOrDefaultAsync(r => r.RoomId == roomId);
                if (room == null)
                {
                    throw new Exception("Room not found");
                }

                // Create a new message object to save to the database
                Message newMessage = new Message
                {
                    UserId = userId,
                    RoomId = roomId,
                    Content = messageContent,
                    Timestamp = DateTime.UtcNow // Consider using UTC for consistency
                };

                // Save the new message to the database
                await _db.Messages.AddAsync(newMessage);
                await _db.SaveChangesAsync();

                // Broadcast the message to all clients in the room

                MessageResponseDTO messageDTO = new()
                {
                    MessageId = newMessage.MessageId,
                    Content = newMessage.Content,
                    UserId = newMessage.UserId,
                    Username = user.Username,
                    RoomId = newMessage.RoomId,
                    Timestamp = newMessage.Timestamp

                };

                await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", messageDTO);

                foreach (var member in room.Users)
                {
                    await Clients.User(member.UserId.ToString()).SendAsync("IncrementPendingMessages", roomId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


    }
}
