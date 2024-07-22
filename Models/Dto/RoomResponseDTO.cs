namespace ChatApp.Models.Dto
{
    public class RoomResponseDTO
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MessageCount { get; set; }
        public int UserCount { get; set; }
        public List<UserDTO> Users { get; set; }
        //public List<MessageResponseDTO> Messages { get; set; }
    }
}
