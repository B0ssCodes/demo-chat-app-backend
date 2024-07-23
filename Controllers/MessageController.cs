using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        [Authorize]
        public async Task<IActionResult> CreateMessage([FromBody] MessageRequestDTO messageDTO)
        {
            ApiResponse<MessageResponseDTO> _response = new();
            try
            {
                // Create message and await the response from the repository
                MessageResponseDTO message = await _messageRepository.CreateMessage(messageDTO);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Message created successfully";
                _response.Result= message;
                return Ok(_response);
            }

            catch (Exception ex)
            {
                // Catch any exception and return a bad request with the error message
                _response.Status = HttpStatusCode.BadRequest;
                _response.Success = false;
                _response.Message = ex.Message;
                _response.Result = null;
                return BadRequest(_response);
            }
            
            
        }

        [HttpGet]
        [Route("room/{roomId}")]
        [Authorize]
        public async Task<IActionResult> GetRoomMessages(int roomId)
        {
            ApiResponse<List<MessageResponseDTO>> _response = new();
            try
            {
                // Get messages based on the roomId fron the repository
                List<MessageResponseDTO> messages = await _messageRepository.GetRoomMessages(roomId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Messages retrieved successfully";
                _response.Result = messages;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Catch any exception and return a not found with the error message
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = ex.Message;
                _response.Result = null;
                return BadRequest(_response);
            }
            
        }

        [HttpGet]
        [Route("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserMessages(int userId)
        {
            ApiResponse<List<MessageResponseDTO>> _response = new();
            try
            {
                // Get messages based on the userId fron the repository
                List<MessageResponseDTO> messages = await _messageRepository.GetUserMessages(userId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Messages retrieved successfully";
                _response.Result = messages;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Catch any exception and return a not found with the error message
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = ex.Message;
                _response.Result = null;
                return BadRequest(_response);
            }
            
        }
    }
}
