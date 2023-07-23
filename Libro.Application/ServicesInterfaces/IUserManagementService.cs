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
        Task<ICollection<UserDTO>> GetAllUsersAsync();
        Task<ICollection<UserDTO>> GetUsersByRoleAsync(UserRole role);
        Task<UserDTO> GetUserByUsernameAsync(string username);
        Task<UserDTO> GetUserByIdAsync(int userId);

        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task<UserDTO> UpdateUserAsync(int userId, UserDTO userDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<UserDTO> AuthenticateUserAsync(string username, string password);
        Task<bool> AssignRoleAsync(string username, UserRole userRole);

        Task<ICollection<BookTransactionDTO>> GetBorrowingHistoryAsync(int patronId);
        Task<ICollection<BookTransactionDTO>> GetCurrentLoansAsync(int patronId);
        Task<ICollection<BookTransactionDTO>> GetOverdueLoansAsync(int patronId);
        Task<ICollection<string>> FindMostFrequentGenresForUserAsync(int userId);
        Task<ICollection<BookDTO>> GetUserRecommendationsAsync(int userId);
    }
}
