using ChatApp.Models;
using ChatApp.Models.Dto;

namespace ChatApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Register(RegisterRequestDTO registerDTOs);
        Task<User> Login(LoginRequestDTO loginDTO);
        Task<UserDetailDTO> GetUserDetails (int userId);
        Task UpdateUserDetails(UserDetailDTO userDetailDTO);
    }
}
