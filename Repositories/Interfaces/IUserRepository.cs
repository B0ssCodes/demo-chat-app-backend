using ChatApp.Models;
using ChatApp.Models.Dto;

namespace ChatApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Register(RegisterRequestDTO registerDTO);
        Task<User> Login(LoginRequestDTO loginDTO);
    }
}
