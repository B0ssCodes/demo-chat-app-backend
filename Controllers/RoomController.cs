using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetRooms()
        {
            ApiResponse<List<RoomResponseDTO>> _response = new ApiResponse<List<RoomResponseDTO>>();
            try
            {
                // Get all rooms from the repository and populate the API response
                List<RoomResponseDTO> roomResponseDTOs = await _roomRepository.GetRooms();
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Rooms retrieved successfully";
                _response.Result = roomResponseDTOs;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // If an error occurs populate the API response with the error message
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }
            

        }

        [HttpGet]
        [Route("getRoom/{roomId}")]
        [Authorize]
        public async Task<IActionResult> GetRoom(int roomId)
        {
            ApiResponse<RoomResponseDTO> _response = new ApiResponse<RoomResponseDTO>();
            try
            {
                // Get the room from the repository and populate the API response
                RoomResponseDTO roomResponseDTO = await _roomRepository.GetRoom(roomId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Room retrieved successfully";
                _response.Result = roomResponseDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // If an error occurs populate the API response with the error message
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }
        }

        [HttpGet]
        [Route("getRoomsByUser/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetRoomsByUser(int userId)
        {
            ApiResponse<List<RoomResponseDTO>> _response = new ApiResponse<List<RoomResponseDTO>>();
            try
            {
                // Get all rooms from the repository and populate the API response
                List<RoomResponseDTO> roomResponseDTOs = await _roomRepository.GetRoomsByUser(userId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Rooms retrieved successfully";
                _response.Result = roomResponseDTOs;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // If an error occurs populate the API response with the error message
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }
        }

        [HttpPost]
        [Route("createRoom")]
        [Authorize]
        public async Task<IActionResult> CreateRoom(RoomRequestDTO roomDTO)
        {
            ApiResponse<RoomResponseDTO> _response = new ApiResponse<RoomResponseDTO>();
            try
            {
                // Create a room in the repository and populate the API response
                RoomResponseDTO roomResponseDTO = await _roomRepository.CreateRoom(roomDTO);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "Room created successfully";
                _response.Result = roomResponseDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // If an error occurs populate the API response with the error message
                _response.Status = HttpStatusCode.Forbidden;
                _response.Success = false;
                _response.Message = $"Error: {ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }

        }

        [HttpPost]
        [Route("addUserToRoom")]
        [Authorize]
        public async Task<IActionResult> AddUserToRoom(RoomJoinDTO roomDTO)
        {
            ApiResponse<RoomResponseDTO> _response = new ApiResponse<RoomResponseDTO>();
            try
            {
                // Add a user to a room in the repository and populate the API response
               RoomResponseDTO roomResponseDTO =  await _roomRepository.AddUserToRoom(roomDTO.UserId, roomDTO.RoomId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "User added to room successfully";
                _response.Result = roomResponseDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // If an error occurs populate the API response with the error message
                _response.Status = HttpStatusCode.Forbidden;
                _response.Success = false;
                _response.Message = $"{ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }

        }

        [HttpPost]
        [Route("removeUserFromRoom")]
        [Authorize]
        public async Task<IActionResult> RemoveUserFromRoom(RoomJoinDTO roomDTO)
        {
            ApiResponse<RoomResponseDTO> _response = new ApiResponse<RoomResponseDTO>();
            try
            {
                // Remove a user from a room in the repository
                await _roomRepository.RemoveUserFromRoom(roomDTO.UserId, roomDTO.RoomId);
                _response.Status = HttpStatusCode.OK;
                _response.Success = true;
                _response.Message = "User removed from room successfully";
                _response.Result = null;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                // If an error occurs populate the API response with the error message
                _response.Status = HttpStatusCode.Forbidden;
                _response.Success = false;
                _response.Message = $"{ex.Message}";
                _response.Result = null;
                return NotFound(_response);
            }

        }
    }
}
