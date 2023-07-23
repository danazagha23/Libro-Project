using Libro.Domain.Entities;
using Libro.Domain.Enums;
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
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int userId);

        Task<bool> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(int userId, User user);
        Task<bool> DeleteUserAsync(int userId);
    }
}
