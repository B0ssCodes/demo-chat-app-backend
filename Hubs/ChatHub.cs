using ChatApp.DataService;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    // Extend the base Hub class
    public class ChatHub : Hub
    {
        // Using dependancy injection to get the MemoryDb instance
        private readonly MemoryDb _memoryDb;
        public ChatHub(MemoryDb memoryDb)
        {
            _memoryDb = memoryDb;
        }
        // This method is called on the initial handshake to the system.
        public async Task JoinChat(UserConnection userConnection)
        {
            await Clients.All.SendAsync("ReceiveMessage", "admin", $"{userConnection.Username} has joined");
        }

        // This method is called when a user joins a specific chat room. It will add the user to the group and send a message to the group. 
        public async Task JoinSpecificChatRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);
            _memoryDb.connections.TryAdd(Context.ConnectionId, userConnection);
            await Clients.Group(userConnection.ChatRoom)
                .SendAsync("JoinSpecificChatRoom", "System", $"{userConnection.Username} has joined room {userConnection.ChatRoom}");
        }

        public async Task SendMessage(string message)
        {
            // if the user is connected to a room, send the message to the room
            if (_memoryDb.connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.ChatRoom).SendAsync("SendMessages", conn.Username, message);
            }
        }

        public async Task IncreaseCookie()
        {
            if (_memoryDb.connections.TryGetValue(Context.ConnectionId, out UserConnection conn))
            {
                await Clients.Group(conn.ChatRoom).SendAsync("IncreaseCookie");
            }
        }

    }
}
