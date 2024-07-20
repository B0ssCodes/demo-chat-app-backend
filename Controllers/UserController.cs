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

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Use the secret key from appsettings.json
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
            // ADD MORE CLAIMS
        }),
                Expires = DateTime.UtcNow.AddDays(14), // Token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Append the token to the response
            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "Login successful";
            _response.Result = new LoginResponseDTO
            {
                // Assuming LoginResponseDTO can hold a token, otherwise adjust accordingly
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

            RegisterResponseDTO responseDTO = new RegisterResponseDTO
            {
                Username = user.Username,
                Email = user.Email,
                Description = user.Description
            };
            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "Register successful";
            _response.Result = responseDTO;
            return Ok(_response);
        }

        [HttpGet]
        [Route("UserDetails/{userId}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            UserDetailDTO userDetails = await _userRepository.GetUserDetails(userId);
            ApiResponse<UserDetailDTO> _response = new ApiResponse<UserDetailDTO>();
            if (userDetails == null)
            {
                _response.Status = HttpStatusCode.NotFound;
                _response.Success = false;
                _response.Message = "User not found";
                _response.Result = null;
                return NotFound(_response);
            }

            _response.Status = HttpStatusCode.OK;
            _response.Success = true;
            _response.Message = "User details found";
            _response.Result = userDetails;
            return Ok(_response);
            
        }


    }
}
