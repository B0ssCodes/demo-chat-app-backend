using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;

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

        public Task<List<MessageResponseDTO>> GetRoomMessages(int roomId)
        {
            var messages = _db.Messages.Where(m => m.RoomId == roomId).ToList();

            return Task.FromResult(messages.Select(m => new MessageResponseDTO
            {
                MessageId = m.MessageId,
                Content = m.Content,
                UserId = m.UserId,
                RoomId = m.RoomId,
                Timestamp = m.Timestamp
            }).ToList());
        }

        public Task<List<MessageResponseDTO>> GetUserMessages(int userId)
        {
            var messages = _db.Messages.Where(m => m.UserId == userId).ToList();

            return Task.FromResult(messages.Select(m => new MessageResponseDTO
            {
                MessageId = m.MessageId,
                Content = m.Content,
                UserId = m.UserId,
                RoomId = m.RoomId,
                Timestamp = m.Timestamp
            }).ToList());
        }
    }
}
