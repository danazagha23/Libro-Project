using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Infrastructure.Data.DbContexts;
using Libro.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Tests.System.Repositories
{
    public class UserRepositoryTests
    {
        private UserRepository _userRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<UserRepository>> _logger;

        public UserRepositoryTests()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<UserRepository>>();

            _userRepository = new UserRepository(_dbContext, _logger.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" },
                new User { UserId = 2, Username = "user2", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" },
                new User { UserId = 3, Username = "user3", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" } 
            };

            _dbContext.Users.AddRange(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllUsersAsync();

            // Assert
            Assert.Equal(users.Count, result.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserWhenExists()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdAsync(user.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUserWhenExists()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByUsernameAsync(user.Username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetUsersByRoleAsync_ShouldReturnUsersWithMatchingRole()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Username = "user1", Role = UserRole.Administrator, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" } ,
                new User { UserId = 2, Username = "user2", Role = UserRole.Patron, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" },
                new User { UserId = 3, Username = "user3", Role = UserRole.Librarian, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" },
                new User { UserId = 4, Username = "user4", Role = UserRole.Administrator , Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" }
            };

            _dbContext.Users.AddRange(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUsersByRoleAsync(UserRole.Patron);

            // Assert
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateNewUser()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };

            // Act
            await _userRepository.CreateUserAsync(user);

            // Assert
            var createdUser = await _dbContext.Users.FindAsync(user.UserId);
            Assert.NotNull(createdUser);
            Assert.Equal(user.Username, createdUser.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateExistingUser()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Update the user's username
            var updatedUsername = "updated_user1";
            user.Username = updatedUsername;

            // Act
            await _userRepository.UpdateUserAsync(user.UserId, user);

            // Assert
            var updatedUser = await _dbContext.Users.FindAsync(user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Equal(updatedUsername, updatedUser.Username);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteExistingUser()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.DeleteUserAsync(user.UserId);

            // Assert
            Assert.True(result);
            var deletedUser = await _dbContext.Users.FindAsync(user.UserId);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task GetBorrowingHistoryAsync_ShouldReturnBorrowingHistoryForPatron()
        {
            // Arrange
            var patronId = 1;
            var patron = new User { UserId = 1, Username = "user1", Role = UserRole.Administrator, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } },
                new Book { BookId = 2, Title = "Book 2", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } },
                new Book { BookId = 3, Title = "Book 3", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } }
            };
            var transactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = patronId, TransactionType = TransactionType.Borrowed, Patron = patron , BookId = 1, Book = books[0]},
                new BookTransaction { TransactionId = 2, PatronId = patronId, TransactionType = TransactionType.Borrowed, Patron = patron , BookId = 2, Book = books[1]},
                new BookTransaction { TransactionId = 3, PatronId = patronId, TransactionType = TransactionType.Borrowed, Patron = patron , BookId = 3, Book = books[2]}
            };

            _dbContext.BookTransactions.AddRange(transactions);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetBorrowingHistoryAsync(patronId);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All(result, transaction => Assert.Equal(patronId, transaction.PatronId));
            Assert.All(result, transaction => Assert.Equal(TransactionType.Borrowed, transaction.TransactionType));
        }

        [Fact]
        public async Task GetCurrentLoansAsync_ShouldReturnCurrentLoansForPatron()
        {
            // Arrange
            var patronId = 1;
            var patron = new User { UserId = 1, Username = "user1", Role = UserRole.Administrator, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } },
                new Book { BookId = 2, Title = "Book 2", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } },
                new Book { BookId = 3, Title = "Book 3", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } }
            };
            var transactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = patronId, TransactionType = TransactionType.Borrowed, IsReturned = true, Patron = patron , BookId = 1, Book = books[0]},
                new BookTransaction { TransactionId = 2, PatronId = patronId, TransactionType = TransactionType.Borrowed, IsReturned = false, Patron = patron , BookId = 2, Book = books[1] },
                new BookTransaction { TransactionId = 3, PatronId = patronId, TransactionType = TransactionType.Borrowed, IsReturned = false, Patron = patron , BookId = 3, Book = books[2] }
            };

            _dbContext.BookTransactions.AddRange(transactions);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetCurrentLoansAsync(patronId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, transaction => Assert.Equal(patronId, transaction.PatronId));
            Assert.All(result, transaction => Assert.Equal(TransactionType.Borrowed, transaction.TransactionType));
            Assert.All(result, transaction => Assert.False(transaction.IsReturned));
        }

        [Fact]
        public async Task GetOverdueLoansAsync_ShouldReturnOverdueLoansForPatron()
        {
            // Arrange
            var patronId = 1;
            var patron = new User { UserId = 1, Username = "user1", Role = UserRole.Administrator, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } },
                new Book { BookId = 2, Title = "Book 2", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } },
                new Book { BookId = 3, Title = "Book 3", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } }
            };
            var currentDate = DateTime.Now;
            var transactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = patronId, TransactionType = TransactionType.Borrowed, IsReturned = false, DueDate = currentDate.AddDays(-3), Patron = patron , BookId = 1, Book = books[0]},
                new BookTransaction { TransactionId = 2, PatronId = patronId, TransactionType = TransactionType.Borrowed, IsReturned = false, DueDate = currentDate.AddDays(-2), Patron = patron , BookId = 2, Book = books[1] },
                new BookTransaction { TransactionId = 3, PatronId = patronId, TransactionType = TransactionType.Borrowed, IsReturned = false, DueDate = currentDate.AddDays(1), Patron = patron , BookId = 3, Book = books[2] }
            };

            _dbContext.BookTransactions.AddRange(transactions);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetOverdueLoansAsync(patronId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, transaction => Assert.Equal(patronId, transaction.PatronId));
            Assert.All(result, transaction => Assert.Equal(TransactionType.Borrowed, transaction.TransactionType));
            Assert.All(result, transaction => Assert.False(transaction.IsReturned));
            Assert.All(result, transaction => Assert.True(transaction.DueDate < currentDate));
        }
    }
}
