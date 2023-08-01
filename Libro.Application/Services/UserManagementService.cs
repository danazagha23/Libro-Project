using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Libro.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IReadingListRepository _readingListRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(IUserRepository userRepository, IBookRepository bookRepository, IReadingListRepository readingListRepository, IValidationService validationService, IMapper mapper, IGenreRepository genreRepository, ILogger<UserManagementService> logger)
        {
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _readingListRepository = readingListRepository;
            _validationService = validationService;
            _mapper = mapper;
            _genreRepository = genreRepository;
            _logger = logger;
        }

        public async Task<ICollection<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                var usersDTO = _mapper.Map<ICollection<UserDTO>>(users);

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all users.");
                throw;
            }
        }

        public async Task<UserDTO> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                var userDTO = _mapper.Map<UserDTO>(user);

                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting user by username: {username}");
                throw;
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                var userDTO = _mapper.Map<UserDTO>(user);

                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting user by ID: {userId}");
                throw;
            }
        }

        public async Task<ICollection<UserDTO>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleAsync(role);
                var usersDTO = _mapper.Map<ICollection<UserDTO>>(users);

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting users by role: {role}");
                throw;
            }
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                {
                    throw new ArgumentNullException(nameof(userDTO));
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a user.");
                throw;
            }
        }

        public async Task<UserDTO> UpdateUserAsync(int userId, UserDTO userDTO)
        {
            try
            {
                // Validation
                _validationService.ValidatePassword(userDTO.Password);
                _validationService.ValidateEmail(userDTO.Email);

                var user = _mapper.Map<User>(userDTO);
                await _userRepository.UpdateUserAsync(userId, user);

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating user with ID: {userId}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                return await _userRepository.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting user with ID: {userId}");
                throw;
            }
        }

        public async Task<UserDTO> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null || !VerifyPassword(password, user.Password))
                {
                    throw new Exception("Invalid username or password.");
                }
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while authenticating the user.");
                throw;
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Hash the provided password
                string hashedInput = HashPassword(password);

                return string.Equals(hashedInput, hashedPassword);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while assigning role '{userRole}' to user '{username}'.");
                throw;
            }
        }

        public async Task<ICollection<BookTransactionDTO>> GetBorrowingHistoryAsync(int patronId)
        {
            try
            {
                var borrowingHistory = await _userRepository.GetBorrowingHistoryAsync(patronId);

                return _mapper.Map<ICollection<BookTransactionDTO>>(borrowingHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting borrowing history for user with ID: {patronId}");
                throw;
            }
        }

        public async Task<ICollection<BookTransactionDTO>> GetCurrentLoansAsync(int patronId)
        {
            try
            {
                var currentLoans = await _userRepository.GetCurrentLoansAsync(patronId);

                return _mapper.Map<ICollection<BookTransactionDTO>>(currentLoans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting current loans for user with ID: {patronId}");
                throw;
            }
        }

        public async Task<ICollection<BookTransactionDTO>> GetOverdueLoansAsync(int patronId)
        {
            try
            {
                var overdueLoans = await _userRepository.GetOverdueLoansAsync(patronId);

                return _mapper.Map<ICollection<BookTransactionDTO>>(overdueLoans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting overdue loans for user with ID: {patronId}");
                throw;
            }
        }

        public async Task<ICollection<string>> FindMostFrequentGenresForUserAsync(int userId)
        {
            try
            {
                var allGenres = await _genreRepository.GetAllGenresAsync();
                var genres = new List<string>();
                var genreCounter = new Dictionary<string, int>();
                var bookTransactions = await _userRepository.GetBorrowingHistoryAsync(userId);
                if (bookTransactions != null && bookTransactions.Any())
                {
                    foreach (var transaction in bookTransactions)
                    {
                        var book = transaction.Book;
                        if (!genreCounter.ContainsKey(book.Genre.Name))
                        {
                            genreCounter[book.Genre.Name] = 1;
                        }
                        else
                        {
                            genreCounter[book.Genre.Name]++;
                        }
                    }
                    int maxCount = 0;
                    if (genreCounter.Values.Any())
                    {
                        maxCount = genreCounter.Values.Max();
                    }
                    genres = genreCounter.Where(kv => kv.Value == maxCount).Select(kv => kv.Key).ToList();
                }
                return genres;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while finding most frequent genres for user with ID: {userId}");
                throw;
            }
        }

        public async Task<ICollection<BookDTO>> GetUserRecommendationsAsync(int userId)
        {
            try
            {
                var genres = await FindMostFrequentGenresForUserAsync(userId);
                var books = await _bookRepository.GetAllBooksAsync();
                var bookDTOs = _mapper.Map<ICollection<BookDTO>>(books);

                var recommendationBooks = bookDTOs.Where(book => genres.Contains(book.Genre.Name));

                return recommendationBooks.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting user recommendations for user with ID: {userId}");
                throw;
            }
        }
    }
}
