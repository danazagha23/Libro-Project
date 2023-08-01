using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Services
{
    public class ReadingListServiceTests
    {
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReadingListService>> _loggerMock;
        private readonly ReadingListService _readingListService;

        public ReadingListServiceTests()
        {
            _readingListRepositoryMock = new Mock<IReadingListRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReadingListService>>();
            _readingListService = new ReadingListService
                (_readingListRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
                );
        }

        [Fact]
        public async Task GetReadingListByIdAsync_WithValidReadingListId_ShouldReturnReadingList()
        {
            // Arrange
            var readingListId = 1;
            var readingList = new ReadingList { Id = readingListId };
            var readingListDTO = new ReadingListDTO { Id = readingListId };

            _readingListRepositoryMock.Setup(repo => repo.GetReadingListByIdAsync(readingListId)).ReturnsAsync(readingList);
            _mapperMock.Setup(mapper => mapper.Map<ReadingListDTO>(readingList)).Returns(readingListDTO);

            // Act
            var result = await _readingListService.GetReadingListByIdAsync(readingListId);

            // Assert
            Assert.Equal(readingListDTO, result);

            _readingListRepositoryMock.Verify(repo => repo.GetReadingListByIdAsync(readingListId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ReadingListDTO>(readingList), Times.Once);
        }

        [Fact]
        public async Task GetReadingListsByUserIdAsync_WithValidUserId_ShouldReturnReadingLists()
        {
            // Arrange
            var userId = 1;
            var readingLists = new List<ReadingList>
            {
                new ReadingList { Id = 1, UserId = userId },
                new ReadingList { Id = 2, UserId = userId }
            };
            var readingListDTOs = new List<ReadingListDTO>
            {
                new ReadingListDTO { Id = 1, UserId = userId },
                new ReadingListDTO { Id = 2, UserId = userId }
            };

            _readingListRepositoryMock.Setup(repo => repo.GetReadingListsByUserIdAsync(userId)).ReturnsAsync(readingLists);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<ReadingListDTO>>(readingLists)).Returns(readingListDTOs);

            // Act
            var result = await _readingListService.GetReadingListsByUserIdAsync(userId);

            // Assert
            Assert.Equal(readingListDTOs, result);

            _readingListRepositoryMock.Verify(repo => repo.GetReadingListsByUserIdAsync(userId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<ReadingListDTO>>(readingLists), Times.Once);
        }

        [Fact]
        public async Task CreateReadingListAsync_WithValidUserId_ShouldCreateReadingList()
        {
            // Arrange
            var userId = 1;
            var readingList = new ReadingList { Id = 1, UserId = userId };
            var readingListDTO = new ReadingListDTO { Id = 1, UserId = userId };

            _readingListRepositoryMock.Setup(repo => repo.CreateReadingListAsync(It.IsAny<ReadingList>())).ReturnsAsync(readingList);
            _mapperMock.Setup(mapper => mapper.Map<ReadingListDTO>((It.IsAny<ReadingList>()))).Returns(readingListDTO);

            // Act
            var result = await _readingListService.CreateReadingListAsync(userId);

            // Assert
            Assert.Equal(readingListDTO, result);

            _readingListRepositoryMock.Verify(repo => repo.CreateReadingListAsync(It.IsAny<ReadingList>()), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ReadingListDTO>(readingList), Times.Once);
        }

        [Fact]
        public async Task AddBookToReadingListAsync_WithValidReadingListIdAndBookId_ShouldAddBookToReadingList()
        {
            // Arrange
            var readingListId = 1;
            var bookId = 1;

            _readingListRepositoryMock.Setup(repo => repo.AddBookToReadingListAsync(readingListId, bookId)).ReturnsAsync(true);

            // Act
            var result = await _readingListService.AddBookToReadingListAsync(readingListId, bookId);

            // Assert
            Assert.True(result);

            _readingListRepositoryMock.Verify(repo => repo.AddBookToReadingListAsync(readingListId, bookId), Times.Once);
        }

        [Fact]
        public async Task RemoveBookFromReadingListAsync_WithValidReadingListIdAndBookId_ShouldRemoveBookFromReadingList()
        {
            // Arrange
            var readingListId = 1;
            var bookId = 1;

            _readingListRepositoryMock.Setup(repo => repo.RemoveBookFromReadingListAsync(readingListId, bookId)).ReturnsAsync(true);

            // Act
            var result = await _readingListService.RemoveBookFromReadingListAsync(readingListId, bookId);

            // Assert
            Assert.True(result);

            _readingListRepositoryMock.Verify(repo => repo.RemoveBookFromReadingListAsync(readingListId, bookId), Times.Once);
        }

        [Fact]
        public async Task IsBookInReadingListAsync_WithValidReadingListIdAndBookId_ShouldReturnTrueIfBookIsInReadingList()
        {
            // Arrange
            var readingListId = 1;
            var bookId = 1;
            var readingList = new ReadingList { Id = readingListId, Books = new List<Book> { new Book { BookId = bookId } } };

            _readingListRepositoryMock.Setup(repo => repo.GetReadingListByIdAsync(readingListId)).ReturnsAsync(readingList);

            // Act
            var result = await _readingListService.IsBookInReadingListAsync(readingListId, bookId);

            // Assert
            Assert.True(result);

            _readingListRepositoryMock.Verify(repo => repo.GetReadingListByIdAsync(readingListId), Times.Once);
        }

        [Fact]
        public async Task IsBookInReadingListAsync_WithValidReadingListIdAndBookId_ShouldReturnFalseIfBookIsNotInReadingList()
        {
            // Arrange
            var readingListId = 1;
            var bookId = 1;
            var readingList = new ReadingList { Id = readingListId, Books = new List<Book>() };

            _readingListRepositoryMock.Setup(repo => repo.GetReadingListByIdAsync(readingListId)).ReturnsAsync(readingList);

            // Act
            var result = await _readingListService.IsBookInReadingListAsync(readingListId, bookId);

            // Assert
            Assert.False(result);

            _readingListRepositoryMock.Verify(repo => repo.GetReadingListByIdAsync(readingListId), Times.Once);
        }
    }
}
