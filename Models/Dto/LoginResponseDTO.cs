﻿namespace ChatApp.Models.Dto
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }

    }
}
