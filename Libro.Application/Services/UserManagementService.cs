using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Libro.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        public UserManagementService(IUserRepository userRepository, IValidationService validationService, IMapper mapper)
        {
            _userRepository = userRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return usersDTO;
        }

        public async Task<UserDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            var userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return usersDTO;
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDTO)
        {
            // Validation
            await _validationService.ValidateUsernameAsync(userDTO.Username, userDTO.UserId);
            _validationService.ValidatePassword(userDTO.Password);
            _validationService.ValidateEmail(userDTO.Email);

            var user = _mapper.Map<User>(userDTO);

            // Hash the provided password
            string hashedPassword = HashPassword(userDTO.Password);
            user.Password = hashedPassword;

            await _userRepository.CreateUserAsync(user);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserAsync(int userId, UserDTO userDTO)
        {
            // Validation
            _validationService.ValidatePassword(userDTO.Password);
            _validationService.ValidateEmail(userDTO.Email);

            var user = _mapper.Map<User>(userDTO);
            await _userRepository.UpdateUserAsync(userId, user);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<UserDTO> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                throw new Exception("Invalid username or password.");
            }
            return _mapper.Map<UserDTO>(user);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Hash the provided password
                string hashedInput = HashPassword(password);

                // Compare the hashed password with the stored hashed password
                return string.Equals(hashedInput, hashedPassword);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash bytes to a hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public async Task<bool> AssignRoleAsync(string username, UserRole userRole)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Role = (UserRole)userRole;
            await _userRepository.UpdateUserAsync(user.UserId, user);

            return true;
        }

        public async Task<List<BookTransactionDTO>> GetBorrowingHistoryAsync(int patronId)
        {
            var borrowingHistory = await _userRepository.GetBorrowingHistoryAsync(patronId);

            return _mapper.Map<List<BookTransactionDTO>>(borrowingHistory);
        }

        public async Task<List<BookTransactionDTO>> GetCurrentLoansAsync(int patronId)
        {
            var currentLoans = await _userRepository.GetCurrentLoansAsync(patronId);

            return _mapper.Map<List<BookTransactionDTO>>(currentLoans);
        }

        public async Task<List<BookTransactionDTO>> GetOverdueLoansAsync(int patronId)
        {
            var overdueLoans = await _userRepository.GetOverdueLoansAsync(patronId);

            return _mapper.Map<List<BookTransactionDTO>>(overdueLoans);
        }
    }
}
