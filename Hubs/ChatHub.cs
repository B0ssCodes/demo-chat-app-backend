using ChatApp.DataService;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    // Extend the base Hub class
    public class ChatHub : Hub
    {
        private readonly MemoryDb _memoryDb;
        public ChatHub(MemoryDb memoryDb)
        {
            _memoryDb = memoryDb;
        }
        // Define a method that clients can call to send messages
        public async Task JoinChat(UserConnection userConnection)
        {
            await Clients.All.SendAsync("ReceiveMessage", "admin", $"{userConnection.Username} has joined");
        }

        public async Task JoinSpecificChatRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);
            _memoryDb.connections.TryAdd(Context.ConnectionId, userConnection);
            await Clients.Group(userConnection.ChatRoom)
                .SendAsync("JoinSpecificChatRoom", "System", $"{userConnection.Username} has joined room {userConnection.ChatRoom}");
        }

        public async Task SendMessage(string message)
        {
            // if the the DB already has the connection
            if (_memoryDb.connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.ChatRoom).SendAsync("SendMessages", conn.Username, message);
            }
        }

    }
}
