using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChatApp.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        [HttpGet]
        [Route("getRooms")]
        public async Task<IActionResult> GetRooms()
        {
            ApiResponse<List<RoomResponseDTO>> _response = new ApiResponse<List<RoomResponseDTO>>();
            try
            {
                List<RoomResponseDTO> roomResponseDTOs = await _roomRepository.GetRooms();
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Rooms retrieved successfully";
                _response.Result = roomResponseDTOs;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }
            

        }

        [HttpGet]
        [Route("getRoomsByUser/{userId}")]
        public async Task<IActionResult> GetRoomsByUser(int userId)
        {
            ApiResponse<List<RoomResponseDTO>> _response = new ApiResponse<List<RoomResponseDTO>>();
            try
            {
                List<RoomResponseDTO> roomResponseDTOs = await _roomRepository.GetRoomsByUser(userId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Rooms retrieved successfully";
                _response.Result = roomResponseDTOs;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }
        }

        [HttpPost]
        [Route("createRoom")]
        public async Task<IActionResult> CreateRoom(RoomRequestDTO roomDTO)
        {
            ApiResponse<RoomResponseDTO> _response = new ApiResponse<RoomResponseDTO>();
            try
            {
                RoomResponseDTO roomResponseDTO = await _roomRepository.CreateRoom(roomDTO);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Room created successfully";
                _response.Result = roomResponseDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Status = HttpStatusCode.Forbidden;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }

        }

        [HttpPost]
        [Route("addUserToRoom")]
        public async Task<IActionResult> AddUserToRoom(int userId, int roomId)
        {
            ApiResponse<RoomResponseDTO> _response = new ApiResponse<RoomResponseDTO>();
            try
            {
                await _roomRepository.AddUserToRoom(userId, roomId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "User added to room successfully";
                _response.Result = null;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Status = HttpStatusCode.Forbidden;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }

        }
    }
}
