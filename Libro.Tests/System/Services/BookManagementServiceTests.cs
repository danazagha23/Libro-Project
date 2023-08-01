using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Services
{
    public class BookManagementServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookManagementService>> _loggerMock;
        private readonly BookManagementService _bookManagementService;

        public BookManagementServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookManagementService>>();
            _bookManagementService = new BookManagementService
                (_bookRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
                );
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1" },
                new Book { BookId = 2, Title = "Book 2" }
            };

            _bookRepositoryMock.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(books);

            var expectedBookDTOs = new List<BookDTO>();

            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookDTO>>(books)).Returns(expectedBookDTOs);

            // Act
            var result = await _bookManagementService.GetAllBooksAsync();

            // Assert
            _bookRepositoryMock.Verify(repo => repo.GetAllBooksAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookDTO>>(books), Times.Once);

            Assert.Same(expectedBookDTOs, result);
        }

        [Fact]
        public async Task GetBookByIdAsync_WithValidId_ShouldReturnBook()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { BookId = bookId, Title = "Book 1" };

            _bookRepositoryMock.Setup(repo => repo.GetBookByIdAsync(bookId)).ReturnsAsync(book);

            var expectedBookDTO = new BookDTO { BookId = bookId, Title = "Book 1" };

            _mapperMock.Setup(mapper => mapper.Map<BookDTO>(book)).Returns(expectedBookDTO);

            // Act
            var result = await _bookManagementService.GetBookByIdAsync(bookId);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(bookId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<BookDTO>(book), Times.Once);

            Assert.Same(expectedBookDTO, result);
        }

        [Fact]
        public async Task CreateBookAsync_ShouldCreateBookAndReturnBookDTO()
        {
            // Arrange
            var bookDTO = new BookDTO { Title = "Book 1" };
            var book = new Book { BookId = 1, Title = "Book 1" };

            _mapperMock.Setup(mapper => mapper.Map<Book>(bookDTO)).Returns(book);
            _mapperMock.Setup(mapper => mapper.Map<BookDTO>(book)).Returns(bookDTO);
            _bookRepositoryMock.Setup(repo => repo.CreateBookAsync(book)).ReturnsAsync(book);

            // Act
            var result = await _bookManagementService.CreateBookAsync(bookDTO);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.CreateBookAsync(book), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<BookDTO>(book), Times.Once);

            Assert.Same(bookDTO, result);
        }

        [Fact]
        public async Task CreateBookAuthorAsync_WithValidBookIdAndAuthorId_ShouldCreateBookAuthorAndReturnBookAuthorDTO()
        {
            // Arrange
            var bookId = 1;
            var authorId = 1;
            var bookAuthor = new BookAuthor { BookId = bookId, AuthorId = authorId };

            _bookRepositoryMock.Setup(repo => repo.CreateBookAuthorAsync(bookId, authorId)).ReturnsAsync(bookAuthor);

            var expectedBookAuthorDTO = new BookAuthorDTO { BookId = bookId, AuthorId = authorId };

            _mapperMock.Setup(mapper => mapper.Map<BookAuthorDTO>(bookAuthor)).Returns(expectedBookAuthorDTO);

            // Act
            var result = await _bookManagementService.CreateBookAuthorAsync(bookId, authorId);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.CreateBookAuthorAsync(bookId, authorId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<BookAuthorDTO>(bookAuthor), Times.Once);

            Assert.Same(expectedBookAuthorDTO, result);
        }

        [Fact]
        public async Task UpdateBookAsync_WithValidIdAndBookDTO_ShouldUpdateBookAndReturnBookDTO()
        {
            // Arrange
            var bookId = 1;
            var bookDTO = new BookDTO { BookId = bookId, Title = "Updated Book" };
            var book = new Book { BookId = bookId, Title = "Book 1" };

            _mapperMock.Setup(mapper => mapper.Map<Book>(bookDTO)).Returns(book);
            _mapperMock.Setup(mapper => mapper.Map<BookDTO>(book)).Returns(bookDTO);

            // Act
            var result = await _bookManagementService.UpdateBookAsync(bookId, bookDTO);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(bookId, book), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<BookDTO>(book), Times.Once);

            Assert.Same(bookDTO, result);
        }

        [Fact]
        public async Task DeleteBookAsync_WithValidId_ShouldDeleteBookAndReturnTrue()
        {
            // Arrange
            var bookId = 1;

            _bookRepositoryMock.Setup(repo => repo.DeleteBookAsync(bookId)).ReturnsAsync(true);

            // Act
            var result = await _bookManagementService.DeleteBookAsync(bookId);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.DeleteBookAsync(bookId), Times.Once);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteBookAuthorsByBookIdAsync_WithValidId_ShouldDeleteBookAuthorsAndReturnTrue()
        {
            // Arrange
            var bookId = 1;

            _bookRepositoryMock.Setup(repo => repo.DeleteBookAuthorsByBookIdAsync(bookId)).ReturnsAsync(true);

            // Act
            var result = await _bookManagementService.DeleteBookAuthorsByBookIdAsync(bookId);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.DeleteBookAuthorsByBookIdAsync(bookId), Times.Once);

            Assert.True(result);
        }

        [Fact]
        public async Task FindBooksAsync_WithValidParameters_ShouldReturnSearchResults()
        {
            // Arrange
            var bookGenre = "Genre";
            var searchString = "Book 1";
            var authorName = "Author 1";
            var availabilityStatus = AvailabilityStatus.Available;

            var searchResults = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1" }
            };

            _bookRepositoryMock.Setup(repo => repo.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus.ToString())).ReturnsAsync(searchResults);

            var expectedBookDTOs = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1" }
            };

            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookDTO>>(searchResults)).Returns(expectedBookDTOs);

            // Act
            var result = await _bookManagementService.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus.ToString());

            // Assert
            _bookRepositoryMock.Verify(repo => repo.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus.ToString()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookDTO>>(searchResults), Times.Once);

            // Assert
            Assert.Equal(expectedBookDTOs, result);
        }
    }
}
