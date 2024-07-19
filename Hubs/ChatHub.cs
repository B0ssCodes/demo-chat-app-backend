using ChatApp.Data;
using ChatApp.DataService;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

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
            if (!int.TryParse(userIdString, out int userId))
            {
                throw new Exception("Invalid user ID");
            }

            // Query the database for all rooms the user is a part of
            var userRooms = _db.Rooms.Where(r => r.Users.Any(u => u.UserId == userId));

            // Add the user to each room's SignalR group
            foreach (var room in userRooms)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomId.ToString());
            }

            await base.OnConnectedAsync();
        }
        public async Task SendMessage(int userId, int roomId, string messageContent)
        {
            User user = await _db.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            Room room = await _db.Rooms.FindAsync(roomId);
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
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", newMessage);
        }

    }
}
