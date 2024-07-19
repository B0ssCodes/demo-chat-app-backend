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
        public async Task<User> Login(LoginRequestDTO loginDTO)
        {

            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == loginDTO.Username && u.Password == loginDTO.Password);

            if (user == null)
            {
                return null;
            }
       
            return user;
       
        }

        public async Task<User> Register(RegisterRequestDTO registerDTO)
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
            
            return newUser;
        }
    }
}
