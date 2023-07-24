using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Services
{
    public class UserManagementServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserManagementService _userManagementService;

        public UserManagementServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _readingListRepositoryMock = new Mock<IReadingListRepository>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _validationServiceMock = new Mock<IValidationService>();
            _mapperMock = new Mock<IMapper>();
            _userManagementService = new UserManagementService(_userRepositoryMock.Object, _bookRepositoryMock.Object, _readingListRepositoryMock.Object, _validationServiceMock.Object, _mapperMock.Object, _genreRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1 },
                new User { UserId = 2 }
            };
            var userDTOs = new List<UserDTO>
            {
                new UserDTO { UserId = 1 },
                new UserDTO { UserId = 2 }
            };

            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<UserDTO>>(users)).Returns(userDTOs);

            // Act
            var result = await _userManagementService.GetAllUsersAsync();

            // Assert
            Assert.Equal(userDTOs, result);

            _userRepositoryMock.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<UserDTO>>(users), Times.Once);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WithValidUsername_ShouldReturnUser()
        {
            // Arrange
            var username = "john_doe";
            var user = new User { UserId = 1, Username = username };
            var userDTO = new UserDTO { UserId = 1, Username = username };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDTO>(user)).Returns(userDTO);

            // Act
            var result = await _userManagementService.GetUserByUsernameAsync(username);

            // Assert
            Assert.Equal(userDTO, result);

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<UserDTO>(user), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidUserId_ShouldReturnUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId };
            var userDTO = new UserDTO { UserId = userId };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDTO>(user)).Returns(userDTO);

            // Act
            var result = await _userManagementService.GetUserByIdAsync(userId);

            // Assert
            Assert.Equal(userDTO, result);

            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<UserDTO>(user), Times.Once);
        }

        [Fact]
        public async Task GetUsersByRoleAsync_WithValidRole_ShouldReturnUsers()
        {
            // Arrange
            var role = UserRole.Administrator;
            var users = new List<User>
            {
                new User { UserId = 1, Role = role },
                new User { UserId = 2, Role = role }
            };
            var userDTOs = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Role = role },
                new UserDTO { UserId = 2, Role = role }
            };

            _userRepositoryMock.Setup(repo => repo.GetUsersByRoleAsync(role)).ReturnsAsync(users);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<UserDTO>>(users)).Returns(userDTOs);

            // Act
            var result = await _userManagementService.GetUsersByRoleAsync(role);

            // Assert
            Assert.Equal(userDTOs, result);

            _userRepositoryMock.Verify(repo => repo.GetUsersByRoleAsync(role), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<UserDTO>>(users), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidUserDTO_ShouldCreateUser()
        {
            // Arrange
            var userDTO = new UserDTO { UserId = 1, Username = "john_doe", Password = "password123", Email = "john@example.com" };
            var user = new User { UserId = 1, Username = "john_doe", Password = "password123", Email = "john@example.com" };

            _validationServiceMock.Setup(service => service.ValidateUsernameAsync(userDTO.Username, userDTO.UserId)).Returns(Task.CompletedTask);
            _validationServiceMock.Setup(service => service.ValidatePassword(userDTO.Password));
            _validationServiceMock.Setup(service => service.ValidateEmail(userDTO.Email));
            _mapperMock.Setup(mapper => mapper.Map<User>(It.IsAny<UserDTO>())).Returns(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDTO>(It.IsAny<User>())).Returns(userDTO);
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(user)).ReturnsAsync(true);

            // Act
            var result = await _userManagementService.CreateUserAsync(userDTO);

            // Assert
            Assert.Equal(userDTO, result);

            _validationServiceMock.Verify(service => service.ValidateUsernameAsync(userDTO.Username, userDTO.UserId), Times.Once);
            _validationServiceMock.Verify(service => service.ValidatePassword(userDTO.Password), Times.Once);
            _validationServiceMock.Verify(service => service.ValidateEmail(userDTO.Email), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<User>(userDTO), Times.Once);
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidUserIdAndUserDTO_ShouldUpdateUser()
        {
            // Arrange
            var userId = 1;
            var userDTO = new UserDTO { UserId = userId, Username = "john_doe", Password = "password123", Email = "john@example.com" };
            var user = new User { UserId = userId, Username = "john_doe", Password = "password123", Email = "john@example.com" };

            _validationServiceMock.Setup(service => service.ValidatePassword(userDTO.Password));
            _validationServiceMock.Setup(service => service.ValidateEmail(userDTO.Email));
            _mapperMock.Setup(mapper => mapper.Map<User>(userDTO)).Returns(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDTO>(user)).Returns(userDTO);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(userId, user)).ReturnsAsync(user);

            // Act
            var result = await _userManagementService.UpdateUserAsync(userId, userDTO);

            // Assert
            Assert.Equal(userDTO, result);

            _validationServiceMock.Verify(service => service.ValidatePassword(userDTO.Password), Times.Once);
            _validationServiceMock.Verify(service => service.ValidateEmail(userDTO.Email), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<User>(userDTO), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(userId, user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidUserId_ShouldDeleteUser()
        {
            // Arrange
            var userId = 1;

            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _userManagementService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result);

            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WithValidUsernameAndPassword_ShouldAuthenticateUser()
        {
            // Arrange
            var username = "john_doe";
            var password = "password123";
            var user = new User { UserId = 1, Username = username, Password = HashPassword(password) };
            var userDTO = new UserDTO { UserId = 1, Username = username };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDTO>(user)).Returns(userDTO);

            // Act
            var result = await _userManagementService.AuthenticateUserAsync(username, password);

            // Assert
            Assert.Equal(userDTO, result);

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<UserDTO>(user), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WithInvalidUsername_ShouldThrowException()
        {
            // Arrange
            var username = "john_doe";
            var password = "password123";

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userManagementService.AuthenticateUserAsync(username, password));

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WithInvalidPassword_ShouldThrowException()
        {
            // Arrange
            var username = "john_doe";
            var password = "password123";
            var user = new User { UserId = 1, Username = username, Password = HashPassword("wrong_password") };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userManagementService.AuthenticateUserAsync(username, password));

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        [Fact]
        public async Task AssignRoleAsync_WithValidUsernameAndUserRole_ShouldAssignRole()
        {
            // Arrange
            var username = "john_doe";
            var userRole = UserRole.Administrator;
            var user = new User { UserId = 1, Username = username };

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(user.UserId, user)).ReturnsAsync(user);

            // Act
            var result = await _userManagementService.AssignRoleAsync(username, userRole);

            // Assert
            Assert.True(result);

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user.UserId, user), Times.Once);
        }

        [Fact]
        public async Task AssignRoleAsync_WithInvalidUsername_ShouldThrowException()
        {
            // Arrange
            var username = "john_doe";
            var userRole = UserRole.Administrator;

            _userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userManagementService.AssignRoleAsync(username, userRole));

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
        }

        [Fact]
        public async Task GetBorrowingHistoryAsync_WithValidPatronId_ShouldReturnBorrowingHistory()
        {
            // Arrange
            var patronId = 1;
            var bookTransactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = patronId },
                new BookTransaction { TransactionId = 2, PatronId = patronId }
            };
            var bookTransactionDTOs = new List<BookTransactionDTO>
            {
                new BookTransactionDTO { TransactionId = 1, PatronId = patronId },
                new BookTransactionDTO { TransactionId = 2, PatronId = patronId }
            };

            _userRepositoryMock.Setup(repo => repo.GetBorrowingHistoryAsync(patronId)).ReturnsAsync(bookTransactions);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookTransactionDTO>>(bookTransactions)).Returns(bookTransactionDTOs);

            // Act
            var result = await _userManagementService.GetBorrowingHistoryAsync(patronId);

            // Assert
            Assert.Equal(bookTransactionDTOs, result);

            _userRepositoryMock.Verify(repo => repo.GetBorrowingHistoryAsync(patronId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookTransactionDTO>>(bookTransactions), Times.Once);
        }

        [Fact]
        public async Task GetCurrentLoansAsync_WithValidPatronId_ShouldReturnCurrentLoans()
        {
            // Arrange
            var patronId = 1;
            var bookTransactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = patronId, IsReturned = false },
                new BookTransaction { TransactionId = 2, PatronId = patronId, IsReturned = false }
            };
            var bookTransactionDTOs = new List<BookTransactionDTO>
            {
                new BookTransactionDTO { TransactionId = 1, PatronId = patronId, IsReturned = false },
                new BookTransactionDTO { TransactionId = 2, PatronId = patronId, IsReturned = false }
            };

            _userRepositoryMock.Setup(repo => repo.GetCurrentLoansAsync(patronId)).ReturnsAsync(bookTransactions);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookTransactionDTO>>(bookTransactions)).Returns(bookTransactionDTOs);

            // Act
            var result = await _userManagementService.GetCurrentLoansAsync(patronId);

            // Assert
            Assert.Equal(bookTransactionDTOs, result);

            _userRepositoryMock.Verify(repo => repo.GetCurrentLoansAsync(patronId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookTransactionDTO>>(bookTransactions), Times.Once);
        }

        [Fact]
        public async Task GetOverdueLoansAsync_WithValidPatronId_ShouldReturnOverdueLoans()
        {
            // Arrange
            var patronId = 1;
            var bookTransactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = patronId, DueDate = DateTime.Now.AddDays(-1), IsReturned = false },
                new BookTransaction { TransactionId = 2, PatronId = patronId, DueDate = DateTime.Now.AddDays(-2), IsReturned = false }
            };
            var bookTransactionDTOs = new List<BookTransactionDTO>
            {
                new BookTransactionDTO { TransactionId = 1, PatronId = patronId, DueDate = DateTime.Now.AddDays(-1), IsReturned = false },
                new BookTransactionDTO { TransactionId = 2, PatronId = patronId, DueDate = DateTime.Now.AddDays(-2), IsReturned = false }
            };

            _userRepositoryMock.Setup(repo => repo.GetOverdueLoansAsync(patronId)).ReturnsAsync(bookTransactions);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookTransactionDTO>>(bookTransactions)).Returns(bookTransactionDTOs);

            // Act
            var result = await _userManagementService.GetOverdueLoansAsync(patronId);

            // Assert
            Assert.Equal(bookTransactionDTOs, result);

            _userRepositoryMock.Verify(repo => repo.GetOverdueLoansAsync(patronId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookTransactionDTO>>(bookTransactions), Times.Once);
        }

        [Fact]
        public async Task FindMostFrequentGenresForUserAsync_WithValidUserId_ShouldReturnMostFrequentGenres()
        {
            // Arrange
            var userId = 1;
            var genres = new List<string> { "Fantasy", "Mystery" };
            var mostFreqGenres = new List<string> { "Fantasy" };
            var bookTransactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = userId, Book = new Book { Genre = new Genre { Name = "Fantasy" } } },
                new BookTransaction { TransactionId = 2, PatronId = userId, Book = new Book { Genre = new Genre { Name = "Mystery" } } },
                new BookTransaction { TransactionId = 3, PatronId = userId, Book = new Book { Genre = new Genre { Name = "Fantasy" } } },
            };

            _userRepositoryMock.Setup(repo => repo.GetBorrowingHistoryAsync(userId)).ReturnsAsync(bookTransactions);
            _genreRepositoryMock.Setup(repo => repo.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());

            // Act
            var result = await _userManagementService.FindMostFrequentGenresForUserAsync(userId);

            // Assert
            Assert.Equal(mostFreqGenres, result);

            _userRepositoryMock.Verify(repo => repo.GetBorrowingHistoryAsync(userId), Times.Once);
            _genreRepositoryMock.Verify(repo => repo.GetAllGenresAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUserRecommendationsAsync_WithValidUserId_ShouldReturnUserRecommendations()
        {
            // Arrange
            var userId = 1;
            var genres = new List<string> { "Fantasy" };
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Genre = new Genre { Name = "Fantasy" } },
                new Book { BookId = 2, Title = "Book 2", Genre = new Genre { Name = "Fantasy" } },
                new Book { BookId = 3, Title = "Book 3", Genre = new Genre { Name = "Sci-Fi" } },
            };
            var bookDTOs = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1", Genre = new GenreDTO { Name = "Fantasy" } },
                new BookDTO { BookId = 2, Title = "Book 2", Genre = new GenreDTO { Name = "Fantasy" } },
                new BookDTO { BookId = 3, Title = "Book 3", Genre = new GenreDTO { Name = "Sci-Fi" } },
            };
            var bookTransactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = userId, Book = new Book { Genre = new Genre { Name = "Fantasy" } } },
                new BookTransaction { TransactionId = 2, PatronId = userId, Book = new Book { Genre = new Genre { Name = "Mystery" } } },
                new BookTransaction { TransactionId = 3, PatronId = userId, Book = new Book { Genre = new Genre { Name = "Fantasy" } } },
            };
            var bookRecommendations = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1", Genre = new GenreDTO { Name = "Fantasy" } },
                new BookDTO { BookId = 2, Title = "Book 2", Genre = new GenreDTO { Name = "Fantasy" } }
            };

            _userRepositoryMock.Setup(repo => repo.GetBorrowingHistoryAsync(userId)).ReturnsAsync(bookTransactions);
            _genreRepositoryMock.Setup(repo => repo.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());
            _bookRepositoryMock.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(books);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookDTO>>(books)).Returns(bookDTOs);

            // Act
            var result = await _userManagementService.GetUserRecommendationsAsync(userId);

            // Assert
            Assert.Equal(bookRecommendations.Count, result.Count);

            _bookRepositoryMock.Verify(repo => repo.GetAllBooksAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookDTO>>(books), Times.Once);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
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
    }
}
