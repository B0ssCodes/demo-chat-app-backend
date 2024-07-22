using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Models.Dto;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    // Extend the base Hub class
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        // Override the OnConnectedAsync method to handle new connections
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            // userId and roomId are passed as query parameters in the connection URL
            var userIdString = httpContext.Request.Query["userId"];
            var roomIdString = httpContext.Request.Query["roomId"];

            // Validate the user ID and room ID and parse them to integers
            if (!int.TryParse(userIdString, out int userId) || !int.TryParse(roomIdString, out int roomId))
            {
                throw new Exception("Invalid user ID or room ID");
            }

            // Making sure that the user is a member of the specified room
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
            // Should add validation that the user and room exist. But
            // there is a trusted frontend so we can make it faster
            try
            {
                // Create a new message object to save to the database
                Message newMessage = new Message
                {
                    UserId = userId,
                    RoomId = roomId,
                    Content = messageContent,
                    // Used UTC as the frontend gave errors without it
                    Timestamp = DateTime.UtcNow 
                };

                await _db.Messages.AddAsync(newMessage);
                await _db.SaveChangesAsync();

                string username = await _db.Users
                                        .Where(u => u.UserId == userId)
                                        .Select(u => u.Username)
                                        .FirstOrDefaultAsync();

                MessageResponseDTO messageDTO = new()
                {
                    MessageId = newMessage.MessageId,
                    Content = newMessage.Content,
                    UserId = newMessage.UserId,
                    Username = username, 
                    RoomId = newMessage.RoomId,
                    Timestamp = newMessage.Timestamp
                };

                await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", messageDTO);

                // Increment the pending messages count for the room
                await Clients.Group(roomId.ToString()).SendAsync("IncrementPendingMessages", roomId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
