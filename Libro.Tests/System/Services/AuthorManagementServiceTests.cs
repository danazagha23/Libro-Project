using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Services
{
    public class AuthorManagementServiceTests
    {
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthorManagementService _authorManagementService;
        private readonly Mock<ILogger<AuthorManagementService>> _loggerMock;

        public AuthorManagementServiceTests()
        {
            _authorRepositoryMock = new Mock<IAuthorRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AuthorManagementService>>();
            _authorManagementService = new AuthorManagementService
                (_authorRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
                );
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ShouldReturnAllAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { AuthorId = 1, AuthorName = "Author 1" },
                new Author { AuthorId = 2, AuthorName = "Author 2" }
            };

            _authorRepositoryMock.Setup(repo => repo.GetAllAuthorsAsync()).ReturnsAsync(authors);

            var expectedAuthorDTOs = new List<AuthorDTO>();

            _mapperMock.Setup(mapper => mapper.Map<ICollection<AuthorDTO>>(authors)).Returns(expectedAuthorDTOs);

            // Act
            var result = await _authorManagementService.GetAllAuthorsAsync();

            // Assert
            _authorRepositoryMock.Verify(repo => repo.GetAllAuthorsAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<AuthorDTO>>(authors), Times.Once);

            Assert.Same(expectedAuthorDTOs, result);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_WithValidId_ShouldReturnAuthor()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, AuthorName = "Author 1" };

            _authorRepositoryMock.Setup(repo => repo.GetAuthorByIdAsync(authorId)).ReturnsAsync(author);

            var expectedAuthorDTO = new AuthorDTO { AuthorId = authorId, AuthorName = "Author 1" };

            _mapperMock.Setup(mapper => mapper.Map<AuthorDTO>(author)).Returns(expectedAuthorDTO);

            // Act
            var result = await _authorManagementService.GetAuthorByIdAsync(authorId);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.GetAuthorByIdAsync(authorId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<AuthorDTO>(author), Times.Once);

            Assert.Same(expectedAuthorDTO, result);
        }

        [Fact]
        public async Task GetAuthorByNameAsync_WithValidName_ShouldReturnAuthor()
        {
            // Arrange
            var authorName = "Author 1";
            var author = new Author { AuthorId = 1, AuthorName = authorName };

            _authorRepositoryMock.Setup(repo => repo.GetAuthorByNameAsync(authorName)).ReturnsAsync(author);

            var expectedAuthorDTO = new AuthorDTO { AuthorId = 1, AuthorName = authorName };

            _mapperMock.Setup(mapper => mapper.Map<AuthorDTO>(author)).Returns(expectedAuthorDTO);

            // Act
            var result = await _authorManagementService.GetAuthorByNameAsync(authorName);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.GetAuthorByNameAsync(authorName), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<AuthorDTO>(author), Times.Once);

            Assert.Same(expectedAuthorDTO, result);
        }

        [Fact]
        public async Task CreateAuthorAsync_ShouldCreateAuthorAndReturnAuthorDTO()
        {
            // Arrange
            var authorDTO = new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" };
            var author = new Author { AuthorId = 1, AuthorName = "Author 1" };

            _mapperMock.Setup(mapper => mapper.Map<Author>(authorDTO)).Returns(author);
            _mapperMock.Setup(mapper => mapper.Map<AuthorDTO>(author)).Returns(authorDTO);
            _authorRepositoryMock.Setup(repo => repo.CreateAuthorAsync(author)).ReturnsAsync(author);

            // Act
            var result = await _authorManagementService.CreateAuthorAsync(authorDTO);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.CreateAuthorAsync(author), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<AuthorDTO>(author), Times.Once);

            Assert.Same(authorDTO, result);
        }

        [Fact]
        public async Task UpdateAuthorAsync_WithValidIdAndAuthorDTO_ShouldUpdateAuthorAndReturnAuthorDTO()
        {
            // Arrange
            var authorId = 1;
            var authorDTO = new AuthorDTO { AuthorId = authorId, AuthorName = "Updated Author" };
            var author = new Author { AuthorId = authorId, AuthorName = "Author 1" };

            _mapperMock.Setup(mapper => mapper.Map<Author>(authorDTO)).Returns(author);
            _mapperMock.Setup(mapper => mapper.Map<AuthorDTO>(author)).Returns(authorDTO);

            // Act
            var result = await _authorManagementService.UpdateAuthorAsync(authorId, authorDTO);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.UpdateAuthorAsync(authorId, author), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<AuthorDTO>(author), Times.Once);

            Assert.Same(authorDTO, result);
        }

        [Fact]
        public async Task DeleteAuthorAsync_WithValidId_ShouldDeleteAuthorAndReturnTrue()
        {
            // Arrange
            var authorId = 1;

            _authorRepositoryMock.Setup(repo => repo.DeleteAuthorAsync(authorId)).ReturnsAsync(true);

            // Act
            var result = await _authorManagementService.DeleteAuthorAsync(authorId);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.DeleteAuthorAsync(authorId), Times.Once);

            Assert.True(result);
        }
    }
}
