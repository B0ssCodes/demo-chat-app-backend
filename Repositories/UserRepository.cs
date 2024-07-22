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

        public async Task<UserDetailDTO> GetUserDetails(int userId)
        {
            var userDetails = await _db.Users
                .Include(u => u.Rooms)
                    .ThenInclude(r => r.Messages)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userDetails == null)
            {
                throw new Exception("User not found");
            }

            UserDetailDTO userDetailDTO = new()
            {
                UserId = userDetails.UserId,
                Username = userDetails.Username,
                Email = userDetails.Email,
                Description = userDetails.Description,
            };

            return userDetailDTO;
        }

        public async Task UpdateUserDetails(UserDetailDTO userDetailDTO)
        {
            User user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userDetailDTO.UserId);
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            // Check if the username already exists for another user
            bool usernameExists = await _db.Users.AnyAsync(u => u.UserId != userDetailDTO.UserId && u.Username == userDetailDTO.Username);
            if (usernameExists)
            {
                throw new Exception("Username already exists");
            }

            // Check if the email already exists for another user
            bool emailExists = await _db.Users.AnyAsync(u => u.UserId != userDetailDTO.UserId && u.Email == userDetailDTO.Email);
            if (emailExists)
            {
                throw new Exception("Email already exists");
            }

            user.Username = userDetailDTO.Username;
            user.Email = userDetailDTO.Email;
            user.Description = userDetailDTO.Description;

            await _db.SaveChangesAsync();
        }
    }
}
