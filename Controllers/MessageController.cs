using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateMessage([FromBody] MessageRequestDTO messageDTO)
        {
            var message = await _messageRepository.CreateMessage(messageDTO);
            return Ok(message);
        }

        [HttpGet]
        [Route("room/{roomId}")]
        public async Task<IActionResult> GetRoomMessages(int roomId)
        {
            var messages = await _messageRepository.GetRoomMessages(roomId);
            return Ok(messages);
        }

        [HttpGet]
        [Route("user/{userId}")]
        public async Task<IActionResult> GetUserMessages(int userId)
        {
            var messages = await _messageRepository.GetUserMessages(userId);
            return Ok(messages);
        }
    }
}
