namespace ChatApp.Models
{
    // This class is used to store the user's connection information
    public class UserConnection
    {
        // Connected user's username
        public string Username { get; set; }

        // The chatroom that the user wants to join
        public string ChatRoom { get; set; }
    }
}
