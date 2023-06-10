using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.Enums;

namespace Libro.Application.ServicesInterfaces
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role);
        Task<UserDTO> GetUserByUsernameAsync(string username);
        Task<UserDTO> GetUserByIdAsync(int userId);

        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task<UserDTO> UpdateUserAsync(int userId, UserDTO userDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
