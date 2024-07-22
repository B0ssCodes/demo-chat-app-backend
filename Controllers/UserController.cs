using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChatApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;


        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
        {
            // Pass the login request to the repository, it will return the user if found
            User user = await _userRepository.Login(loginDTO);
            ApiResponse<LoginResponseDTO> _response = new ApiResponse<LoginResponseDTO>();
            if (user == null)
            {
                _response.Status = HttpStatusCode.Unauthorized;
                _response.Success = false;
                _response.Message = "Incorrect Username/Password";
                _response.Result = null;
                return Unauthorized(_response);
            }

            // Generate a JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Get the key from appssettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            // Add the userId and username to the token
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "Login successful";
            _response.Result = new LoginResponseDTO
            {
                Token = tokenString,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Description = user.Description
            };

            return Ok(_response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDTO)
        {
            // Pass the register request to the repository, it will return the user if successful
            User user = await _userRepository.Register(registerDTO);

            ApiResponse<RegisterResponseDTO> _response = new ApiResponse<RegisterResponseDTO>();

            // If the user is null, the user already exists
            if (user == null)
            {
                _response.Status = HttpStatusCode.AlreadyReported;
                _response.Success = false;
                _response.Message = "User Already Exists";
                _response.Result = null;
                return Conflict(_response);
            }

            // Populate the api response
            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "Register successful";
            _response.Result = null;
            return Ok(_response);
        }

        [HttpGet]
        [Route("getUserDetails/{userId}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            UserDetailDTO userDetails = await _userRepository.GetUserDetails(userId);
            ApiResponse<UserDetailDTO> _response = new ApiResponse<UserDetailDTO>();

            // If the userDetails are null, the user does not exist
            if (userDetails == null)
            {
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = "User not found";
                _response.Result = null;
                return NotFound(_response);
            }
            // Populate the api response
            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "User details found";
            _response.Result = userDetails;
            return Ok(_response);
            
        }

        [HttpPut]
        [Route("updateUserDetails")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UserDetailDTO userDetailDTO)
        {
            try
            {

                await _userRepository.UpdateUserDetails(userDetailDTO);
                ApiResponse<UserDetailDTO> _response = new ApiResponse<UserDetailDTO>
                {
                    Status = HttpStatusCode.OK,
                    Success = true,
                    Message = "User details updated successfully!",
                    Result = userDetailDTO
                };
                return Ok(_response);
            }
            catch (Exception ex)
            {
                ApiResponse<UserDetailDTO> _response = new ApiResponse<UserDetailDTO>
                {
                    Status = HttpStatusCode.NotFound,
                    Success = false,
                    Message = ex.Message,
                    Result = null
                };
                return BadRequest(_response);
            }
        }


    }
}
