using DTOs;

namespace BlazorApp.Services;

public interface IUserService
{
    Task<UserDTO> AddUserAsync(CreateUserDTO request);
    Task UpdateUserAsync(int id, UpdateUserDTO request);
    Task<UserDTO> GetUserAsync(int id);
    Task<IEnumerable<UserDTO>> GetUsersAsync();
    Task DeleteUserAsync(int id);
}