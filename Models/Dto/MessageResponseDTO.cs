﻿namespace ChatApp.Models.Dto
{
    public class MessageResponseDTO
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
