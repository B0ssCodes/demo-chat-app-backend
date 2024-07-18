using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginDTO)
        {

            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == loginDTO.Username && u.Password == loginDTO.Password);

            if (user == null)
            {
                return null;
            }
           
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Username = user.Username,
                Description = user.Description,
                Email = user.Email

            };
            return loginResponseDTO;
       
        }

        public async Task<RegisterResponseDTO> Register(RegisterRequestDTO registerDTO)
        {
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == registerDTO.Username);

            if (user != null)
            {
                return null;
            }

            User newUser = new User
            {
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                Description = registerDTO.Description,
                Password = registerDTO.Password
            };

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            RegisterResponseDTO registerResponseDTO = new RegisterResponseDTO()
            {
                Username = newUser.Username,
                Description = newUser.Description,
                Email = newUser.Email

            };

            
            return registerResponseDTO;
        }
    }
}
