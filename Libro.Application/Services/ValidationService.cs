using Libro.Application.ServicesInterfaces;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(IUserRepository userRepository, ILogger<ValidationService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task ValidateUsernameAsync(string username, int userId)
        {
            try
            {
                // Check if username is taken
                var existingUser = await _userRepository.GetUserByUsernameAsync(username);
                if (existingUser != null && existingUser.UserId != userId)
                {
                    throw new ArgumentException("Username is already taken");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating username.");
                throw;
            }
        }

        public void ValidatePassword(string password)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(password) && password.Length < 8)
                {
                    throw new ArgumentException("Password must have at least 8 characters");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating password.");
                throw;
            }
        }

        public void ValidateEmail(string email)
        {
            try
            {
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(email, emailPattern))
                {
                    throw new ArgumentException("Invalid email format.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating email.");
                throw;
            }
        }
    }
}
