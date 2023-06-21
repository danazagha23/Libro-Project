﻿using Libro.Domain.Entities;
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
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _context.Users.Where(u => u.Role == role).ToListAsync();

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

    }
}