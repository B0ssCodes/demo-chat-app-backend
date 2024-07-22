using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;
        public MessageRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<MessageResponseDTO> CreateMessage(MessageRequestDTO messageDTO)
        {
            // Map the DTO to the model
            Message message = new Message
            {
                Content = messageDTO.Content,
                Timestamp = DateTime.UtcNow,
                UserId = messageDTO.UserId,
                RoomId = messageDTO.RoomId
            };
            // Add and save the message to the database
            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();

            // Map the model to the DTO and return it
            return new MessageResponseDTO
            {
                MessageId = message.MessageId,
                Content = message.Content,
                UserId = message.UserId,
                RoomId = message.RoomId,
                Timestamp = message.Timestamp
            };
        }

        public async Task<List<MessageResponseDTO>> GetRoomMessages(int roomId)
        {
            // Get all the messages based on the roomId
            var messages = await _db.Messages
                                    .Include(m => m.User)
                                    .Where(m => m.RoomId == roomId)
                                    .OrderBy(m => m.Timestamp)
                                    .ToListAsync();
            
            if (messages == null || !messages.Any())
            {
                throw new Exception("No messages for this room found!");
            }

            // Map the messages to MessageResponseDTOs
            var messageDTOs = messages.Select(m => new MessageResponseDTO
            {
                MessageId = m.MessageId,
                Content = m.Content,
                UserId = m.UserId,
                Username = m.User.Username,
                RoomId = m.RoomId,
                Timestamp = m.Timestamp
            }).ToList();

            return messageDTOs;
        }

        public async Task<List<MessageResponseDTO>> GetUserMessages(int userId)
        {
            // Get all the messages based on the userId
            var messages = await _db.Messages.Include(m => m.User).Where(m => m.UserId == userId).ToListAsync();

            if (messages == null || !messages.Any())
            {
                throw new Exception("No messages for this user found!");
            }

            // Map the messages to MessageResponseDTOs
            var messageDTOs = messages.Select(m => new MessageResponseDTO
            {
                MessageId = m.MessageId,
                Content = m.Content,
                UserId = m.UserId,
                Username = m.User.Username, 
                RoomId = m.RoomId,
                Timestamp = m.Timestamp
            }).ToList();

            return messageDTOs;
        }
    }
}
