using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChatApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;


        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
      
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var user = await _userRepository.Login(loginDTO);
            ApiResponse<LoginResponseDTO> _response = new ApiResponse<LoginResponseDTO>();
            if (user == null)
            {
                _response.Status = HttpStatusCode.Unauthorized;
                _response.Success = false;
                _response.Message = "Incorrect Username/Password";
                _response.Result = null;
                return Unauthorized(_response);
            }

            
            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "Login successful";
            _response.Result = user;

            return Ok(_response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDTO)
        {
            var user = await _userRepository.Register(registerDTO);
            ApiResponse<RegisterResponseDTO> _response = new ApiResponse<RegisterResponseDTO>();
            if (user == null)
            {
                _response.Status = HttpStatusCode.AlreadyReported;
                _response.Success = false;
                _response.Message = "User Already Exists";
                _response.Result = null;
                return Conflict(_response);
            }
            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "Register successful";
            _response.Result = user;
            return Ok(_response);
        }


    }
}
