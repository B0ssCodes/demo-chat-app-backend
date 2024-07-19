namespace ChatApp.Models.Dto
{
    public class MessageRequestDTO
    {
        public string Content { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
