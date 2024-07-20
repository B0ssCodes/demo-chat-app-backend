namespace ChatApp.Models.Dto
{
    public class UserDetailDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public int MessageNumber { get; set; }
        public List<RoomResponseDTO> Rooms { get; set; }
        public List<MessageResponseDTO> Messages { get; set; }
    }
}
