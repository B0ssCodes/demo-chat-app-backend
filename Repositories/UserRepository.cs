using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Models.Dto;
using ChatApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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
            // Search for the user in db
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == loginDTO.Username);

            if (user == null)
            {
                return null; // User not found
            }

            // Retrieve the salt for the user and hash the provided password (copilot)
            byte[] salt = Convert.FromBase64String(user.PasswordSalt);
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: loginDTO.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Compare the hashed password with the stored hash
            if (hashedPassword == user.PasswordHash)
            {
                return user; // Password is correct, return the user to controller
            }
            else
            {
                return null; // Password is incorrect
            }
        }

        public async Task<User> Register(RegisterRequestDTO registerDTO)
        {
            User existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == registerDTO.Username);

            if (existingUser != null)
            {
                return null; // Username already exists
            }

            // Generate a unique salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string saltString = Convert.ToBase64String(salt);

            // Hash the password with the salt
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: registerDTO.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            User newUser = new User
            {
                Username = registerDTO.Username,
                Email = registerDTO.Email,
                Description = registerDTO.Description,
                PasswordHash = hashedPassword,
                PasswordSalt = saltString
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
            // Get the user from the database
            User user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userDetailDTO.UserId);
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            // Check if the username already exists
            bool usernameExists = await _db.Users.AnyAsync(u => u.UserId != userDetailDTO.UserId && u.Username == userDetailDTO.Username);
            if (usernameExists)
            {
                throw new Exception("Username already exists");
            }

            // Check if the email already exists 
            bool emailExists = await _db.Users.AnyAsync(u => u.UserId != userDetailDTO.UserId && u.Email == userDetailDTO.Email);
            if (emailExists)
            {
                throw new Exception("Email already exists");
            }

            // Update the user, no need to Update with LINQ, EF will track the changes
            user.Username = userDetailDTO.Username;
            user.Email = userDetailDTO.Email;
            user.Description = userDetailDTO.Description;

            await _db.SaveChangesAsync();
        }
    }
}
