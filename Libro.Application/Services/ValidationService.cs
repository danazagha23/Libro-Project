using Libro.Application.ServicesInterfaces;
using Libro.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IUserRepository _userRepository;
        public ValidationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateUsernameAsync(string username)
        {
            // Check if username is taken
            var existingUser = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new ArgumentException("Username is already taken");
            }
        }
        public void ValidatePassword(string password)
        {
            // Example: Password must have at least 8 characters
            if(!string.IsNullOrWhiteSpace(password) && password.Length < 8)
            {
                throw new ArgumentException("Password must have at least 8 characters");
            }
        }
        public void ValidateEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                throw new ArgumentException("Invalid email format.");
            }
        }
    }
}
