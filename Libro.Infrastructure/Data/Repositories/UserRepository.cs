using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(LibroDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users from the database.");
                return await _context.Users
                    .Include(u => u.ReadingLists)
                    .Include(u => u.Notifications)
                    .Include(u => u.Reviews)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users.");
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching user by ID: {UserId} from the database.", userId);
                var user = await _context.Users
                    .Include(u => u.ReadingLists)
                    .Include(u => u.Notifications)
                    .Include(u => u.Reviews)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user by ID: {UserId}.", userId);
                throw;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                _logger.LogInformation("Fetching user by username: {Username} from the database.", username);
                var user = await _context.Users
                    .Include(u => u.ReadingLists)
                    .Include(u => u.Notifications)
                    .Include(u => u.Reviews)
                    .FirstOrDefaultAsync(u => u.Username == username);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user by username: {Username}.", username);
                throw;
            }
        }

        public async Task<ICollection<User>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                _logger.LogInformation("Fetching users by role: {Role} from the database.", role);
                var users = await _context.Users
                    .Include(u => u.ReadingLists)
                    .Include(u => u.Notifications)
                    .Include(u => u.Reviews)
                    .Where(u => u.Role == role)
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users by role: {Role}.", role);
                throw;
            }
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new user.");
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(int userId, User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null)
                {
                    throw new Exception("User not found.");
                }

                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();

                return existingUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}.", userId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null)
                {
                    throw new Exception("User not found.");
                }

                _context.Users.Remove(existingUser);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}.", userId);
                throw;
            }
        }

        public async Task<ICollection<BookTransaction>> GetBorrowingHistoryAsync(int patronId)
        {
            try
            {
                _logger.LogInformation("Fetching borrowing history for patron with ID: {PatronId}.", patronId);
                return await _context.BookTransactions
                    .Include(b => b.Book)
                    .Include(p => p.Patron)
                    .Where(t => t.PatronId == patronId && t.TransactionType != TransactionType.Returned)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching borrowing history for patron with ID: {PatronId}.", patronId);
                throw;
            }
        }

        public async Task<ICollection<BookTransaction>> GetCurrentLoansAsync(int patronId)
        {
            try
            {
                _logger.LogInformation("Fetching current loans for patron with ID: {PatronId}.", patronId);
                return await _context.BookTransactions
                    .Include(b => b.Book)
                    .Include(p => p.Patron)
                    .Where(bt => bt.PatronId == patronId && bt.TransactionType == TransactionType.Borrowed && !bt.IsReturned)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching current loans for patron with ID: {PatronId}.", patronId);
                throw;
            }
        }

        public async Task<ICollection<BookTransaction>> GetOverdueLoansAsync(int patronId)
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                _logger.LogInformation("Fetching overdue loans for patron with ID: {PatronId}.", patronId);
                return await _context.BookTransactions
                    .Include(b => b.Book)
                    .Include(p => p.Patron)
                    .Where(bt => bt.PatronId == patronId && bt.TransactionType == TransactionType.Borrowed && !bt.IsReturned && bt.DueDate < currentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching overdue loans for patron with ID: {PatronId}.", patronId);
                throw;
            }
        }
    }
}
