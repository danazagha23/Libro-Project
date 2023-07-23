using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LibroDbContext _context;
        public UserRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.ReadingLists)
                .ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.ReadingLists)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.ReadingLists)
                .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _context.Users
                .Include(u => u.ReadingLists)
                .Where(u => u.Role == role)
                .ToListAsync();

            return users;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User> UpdateUserAsync(int userId, User user)
        {
            var exsistingUser = await _context.Users.FindAsync(userId);
            if (exsistingUser == null)
            {
                throw new Exception("User not Found");
            }
            _context.Entry(exsistingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();

            return exsistingUser;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var exsistingUser = await _context.Users.FindAsync(userId);
            if (exsistingUser == null)
            {
                throw new Exception("User not Found");
            }
            _context.Users.Remove(exsistingUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<BookTransaction>> GetBorrowingHistoryAsync(int patronId)
        {
            return await _context.BookTransactions
                .Include(b => b.Book)
                .Include(p => p.Patron)
                .Where(t => t.PatronId == patronId)
                .ToListAsync();
        }

        public async Task<List<BookTransaction>> GetCurrentLoansAsync(int patronId)
        {
            return await _context.BookTransactions
                .Include(b => b.Book)
                .Include(p => p.Patron)
                .Where(bt => bt.PatronId == patronId && bt.TransactionType == TransactionType.Borrowed && !bt.IsReturned)
                .ToListAsync();
        }

        public async Task<List<BookTransaction>> GetOverdueLoansAsync(int patronId)
        {
            DateTime currentDate = DateTime.Now;
            return await _context.BookTransactions
                .Include(b => b.Book)
                .Include(p => p.Patron)
                .Where(bt => bt.PatronId == patronId && bt.TransactionType == TransactionType.Borrowed && !bt.IsReturned && bt.DueDate < currentDate)
                .ToListAsync();
        }

    }
}
