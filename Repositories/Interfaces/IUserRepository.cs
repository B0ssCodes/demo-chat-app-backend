using ChatApp.Models;
using ChatApp.Models.Dto;

namespace ChatApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<RegisterResponseDTO> Register(RegisterRequestDTO registerDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginDTO);
    }
}
