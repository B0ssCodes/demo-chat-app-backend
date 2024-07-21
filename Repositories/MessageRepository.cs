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
            Message message = new Message
            {
                Content = messageDTO.Content,
                Timestamp = DateTime.UtcNow,
                UserId = messageDTO.UserId,
                RoomId = messageDTO.RoomId
            };

            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();

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
            var messages = await _db.Messages
                                    .Include(m => m.User)
                                    .Where(m => m.RoomId == roomId)
                                    .OrderBy(m => m.Timestamp) 
                                    .Select(m => new MessageResponseDTO
                                    {
                                        MessageId = m.MessageId,
                                        Content = m.Content,
                                        UserId = m.UserId,
                                        Username = m.User.Username,
                                        RoomId = m.RoomId,
                                        Timestamp = m.Timestamp
                                    })
                                    .ToListAsync();

            if (messages == null || !messages.Any())
            {
                throw new Exception("No messages for this room found!");
            }

            return messages;
        }

        public Task<List<MessageResponseDTO>> GetUserMessages(int userId)
        {
            var messages = _db.Messages.Include(m => m.User).Where(m => m.UserId == userId).ToList();

            if (messages == null)
            {
                throw new Exception("No messages for this user found!");
            }

            return Task.FromResult(messages.Select(m => new MessageResponseDTO
            {
                MessageId = m.MessageId,
                Content = m.Content,
                UserId = m.UserId,
                Username = m.User.Username,
                RoomId = m.RoomId,
                Timestamp = m.Timestamp
            }).ToList());
        }
    }
}
