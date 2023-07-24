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
    public class BookTransactionsRepositoryTests
    {
        private BookTransactionsRepository _bookTransactionsRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<BookTransactionsRepository>> _logger;

        public BookTransactionsRepositoryTests()
        {
            // Create a new in-memory database with a unique name for each test case
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<BookTransactionsRepository>>();

            _bookTransactionsRepository = new BookTransactionsRepository(_dbContext, _logger.Object);
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldCreateNewTransaction()
        {
            // Arrange
            var transaction = new BookTransaction
            {
                TransactionType = TransactionType.Borrowed,
                TransactionDate = DateTime.Now,
                BookId = 1,
                PatronId = 1
            };

            // Act
            var result = await _bookTransactionsRepository.CreateTransactionAsync(transaction);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.TransactionId > 0);

            // Check if the transaction is added to the database
            var createdTransaction = await _dbContext.BookTransactions.FindAsync(result.TransactionId);
            Assert.NotNull(createdTransaction);
            Assert.Equal(transaction.TransactionType, createdTransaction.TransactionType);
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAllBookTransactionsAsync_ShouldReturnAllTransactions()
        {
            // Arrange
            var transactions = new List<BookTransaction>
            {
                new BookTransaction
                {
                    TransactionId = 1,
                    TransactionType = TransactionType.Borrowed,
                    TransactionDate = DateTime.Now,
                    BookId = 1,
                    PatronId = 1
                },
                new BookTransaction
                {
                    TransactionId = 2,
                    TransactionType = TransactionType.Borrowed,
                    TransactionDate = DateTime.Now,
                    BookId = 2,
                    PatronId = 2
                }
            };

            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } },
                new Book { BookId = 2, Title = "Book 2", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } }
            };
            var patrons = new List<User>
            {
                new User { UserId = 1, Username = "user1", Role = UserRole.Patron, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678"},
                new User { UserId = 2, Username = "user2", Role = UserRole.Patron, Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678"}
            };

            _dbContext.Books.AddRange(books);
            _dbContext.Users.AddRange(patrons);

            _dbContext.BookTransactions.AddRange(transactions);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _bookTransactionsRepository.GetAllBookTransactionsAsync();

            // Assert
            Assert.Equal(transactions.Count, result.Count);
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetTransactionByIdAsync_ShouldReturnTransactionWhenExists()
        {
            // Arrange
            var transaction = new BookTransaction
            {
                TransactionType = TransactionType.Borrowed,
                TransactionDate = DateTime.Now,
                BookId = 1,
                PatronId = 1
            };

            _dbContext.BookTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _bookTransactionsRepository.GetTransactionByIdAsync(transaction.TransactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transaction.TransactionId, result.TransactionId);
            _dbContext.Dispose();
        }

    }
}
