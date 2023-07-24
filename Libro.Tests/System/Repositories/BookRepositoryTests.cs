using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Libro.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.System.Repositories
{
    public class BookRepositoryTests
    {
        private BookRepository _bookRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<BookRepository>> _logger;

        public BookRepositoryTests()
        {
            // Create a new in-memory database with a unique name for each test case
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<BookRepository>>();

            _bookRepository = new BookRepository(_dbContext, _logger.Object);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } },
                new Book { BookId = 2, Title = "Book 2", Description = "new book desc...", Genre = new Genre { Name = "Non-Fiction" } },
                new Book { BookId = 3, Title = "Book 3", Description = "new book desc...", Genre = new Genre { Name = "Fantasy" } }
            };

            _dbContext.Books.AddRange(books);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _bookRepository.GetAllBooksAsync();

            // Assert
            Assert.Equal(books.Count, result.Count);
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBookWhenExists()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } };

            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _bookRepository.GetBookByIdAsync(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.BookId);
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldThrowExceptionWhenBookNotExists()
        {
            // Arrange
            var nonExistentBookId = 100;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookRepository.GetBookByIdAsync(nonExistentBookId));
            _dbContext.Dispose();
        }

        [Fact]
        public async Task CreateBookAsync_ShouldCreateNewBook()
        {
            // Arrange
            var book = new Book { Title = "New Book", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } };

            // Act
            var result = await _bookRepository.CreateBookAsync(book);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.BookId > 0);

            // Check if the book is added to the database
            var createdBook = await _dbContext.Books.FindAsync(result.BookId);
            Assert.NotNull(createdBook);
            Assert.Equal(book.Title, createdBook.Title);

            _dbContext.Dispose();
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldUpdateExistingBook()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } };
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Update the book title
            var updatedTitle = "Updated Book";
            var updatedBook = new Book { BookId = bookId, Title = updatedTitle, Genre = new Genre { Name = "Fiction" } };

            // Act
            var result = await _bookRepository.UpdateBookAsync(bookId, updatedBook);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedTitle, result.Title);

            // Check if the book is updated in the database
            var updatedBookInDb = await _dbContext.Books.FindAsync(bookId);
            Assert.NotNull(updatedBookInDb);
            Assert.Equal(updatedTitle, updatedBookInDb.Title);
            _dbContext.Dispose();
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldThrowExceptionWhenBookNotExists()
        {
            // Arrange
            var nonExistentBookId = 100;
            var book = new Book { BookId = nonExistentBookId, Title = "NonExistentBook", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookRepository.UpdateBookAsync(nonExistentBookId, book));
            _dbContext.Dispose();
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldDeleteExistingBook()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1", Description = "new book desc...", Genre = new Genre { Name = "Fiction" } };
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _bookRepository.DeleteBookAsync(bookId);

            // Assert
            Assert.True(result);

            // Check if the book is deleted from the database
            var deletedBookInDb = await _dbContext.Books.FindAsync(bookId);
            Assert.Null(deletedBookInDb);
            _dbContext.Dispose();
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldThrowExceptionWhenBookNotExists()
        {
            // Arrange
            var nonExistentBookId = 100;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookRepository.DeleteBookAsync(nonExistentBookId));
            _dbContext.Dispose();
        }

        [Fact]
        public async Task FindBooksAsync_ShouldReturnFilteredBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Harry Potter and the Sorcerer's Stone", Description = "new book desc...", Genre = new Genre { Name = "Fantasy" } },
                new Book { BookId = 2, Title = "1984", Description = "new book desc...", Genre = new Genre { Name = "Science Fiction" } },
                new Book { BookId = 3, Title = "To Kill the Mockingbird", Description = "new book desc...", Genre = new Genre { Name = "Classics" } },
                new Book { BookId = 4, Title = "The Catcher in the Rye", Description = "new book desc...", Genre = new Genre { Name = "Classics" } },
            };

            _dbContext.Books.AddRange(books);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _bookRepository.FindBooksAsync(bookGenre: "Classics", searchString: "the", authorName: "", availabilityStatus: "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Title.Contains("To Kill the Mockingbird"));
            Assert.Contains(result, b => b.Title.Contains("The Catcher in the Rye"));

            _dbContext.Dispose();
        }
    }
}
