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
                MessageNumber = userDetails.Messages.Count,
                Rooms = userDetails.Rooms.Select(r => new RoomResponseDTO
                {
                    RoomId = r.RoomId,
                    Name = r.Name,
                    Description = r.Description,
                    MessageCount = r.Messages.Count,

                }).ToList(),
                Messages = userDetails.Messages.Select(m => new MessageResponseDTO
                {
                    MessageId = m.MessageId,
                    Content = m.Content,
                    RoomId = m.RoomId,
                    UserId = m.UserId,
                    Username = m.User.Username
                }).ToList()
            };


                return userDetailDTO;
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
