using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int userId);

        Task<User> CreateUserAsync(User userDto);
        Task<User> UpdateUserAsync(int userId, User userDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
