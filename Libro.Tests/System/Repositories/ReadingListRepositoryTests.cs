using Libro.Domain.Entities;
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
    public class ReadingListRepositoryTests
    {
        private ReadingListRepository _readingListRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<ReadingListRepository>> _logger;

        public ReadingListRepositoryTests()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<ReadingListRepository>>();

            _readingListRepository = new ReadingListRepository(_dbContext, _logger.Object);
        }

        [Fact]
        public async Task GetReadingListByIdAsync_ShouldReturnReadingListWhenExists()
        {
            // Arrange
            var readingList = new ReadingList { Id = 1 };

            _dbContext.ReadingLists.Add(readingList);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _readingListRepository.GetReadingListByIdAsync(readingList.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(readingList.Id, result.Id);
        }

        [Fact]
        public async Task GetReadingListsByUserIdAsync_ShouldReturnReadingListsForUser()
        {
            // Arrange
            var userId = 1;
            var readingLists = new List<ReadingList>
            {
                new ReadingList { Id = 1, UserId = userId },
                new ReadingList { Id = 2, UserId = userId },
                new ReadingList { Id = 3, UserId = userId }
            };

            _dbContext.ReadingLists.AddRange(readingLists);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _readingListRepository.GetReadingListsByUserIdAsync(userId);

            // Assert
            Assert.Equal(readingLists.Count, result.Count);
            Assert.All(result, rl => Assert.Equal(userId, rl.UserId));
        }

        [Fact]
        public async Task CreateReadingListAsync_ShouldCreateNewReadingList()
        {
            // Arrange
            var readingList = new ReadingList { Id = 1 };

            // Act
            await _readingListRepository.CreateReadingListAsync(readingList);

            // Assert
            var createdReadingList = await _dbContext.ReadingLists.FindAsync(readingList.Id);
            Assert.NotNull(createdReadingList);
            Assert.Equal(readingList.Id, createdReadingList.Id);
        }

        [Fact]
        public async Task AddBookToReadingListAsync_ShouldAddBookToReadingList()
        {
            // Arrange
            var readingListId = 1;
            var bookId = 1;
            var readingList = new ReadingList { Id = readingListId };
            var book = new Book { BookId = bookId, Title = "Book 1", Description = "description..." };

            _dbContext.ReadingLists.Add(readingList);
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _readingListRepository.AddBookToReadingListAsync(readingListId, bookId);

            // Assert
            Assert.True(result);
            var updatedReadingList = await _dbContext.ReadingLists
                .Include(rl => rl.Books)
                .FirstOrDefaultAsync(rl => rl.Id == readingListId);
            Assert.NotNull(updatedReadingList);
            Assert.Contains(updatedReadingList.Books, b => b.BookId == bookId);
        }

        [Fact]
        public async Task RemoveBookFromReadingListAsync_ShouldRemoveBookFromReadingList()
        {
            // Arrange
            var readingListId = 1;
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1", Description = "description..."  };
            var readingList = new ReadingList { Id = readingListId, Books = new List<Book> {  } };

            readingList.Books.Add(book);
            _dbContext.ReadingLists.Add(readingList);
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _readingListRepository.RemoveBookFromReadingListAsync(readingListId, bookId);

            // Assert
            Assert.True(result);
            var updatedReadingList = await _dbContext.ReadingLists
                .Include(rl => rl.Books)
                .FirstOrDefaultAsync(rl => rl.Id == readingListId);
            Assert.NotNull(updatedReadingList);
            Assert.Empty(updatedReadingList.Books); // Ensure the book is removed from the reading list
        }

    }
}
