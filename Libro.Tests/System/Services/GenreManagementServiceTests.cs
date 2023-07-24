using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Services
{
    public class GenreManagementServiceTests
    {
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GenreManagementService _genreManagementService;

        public GenreManagementServiceTests()
        {
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _mapperMock = new Mock<IMapper>();
            _genreManagementService = new GenreManagementService(_genreRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllGenresAsync_ShouldReturnAllGenres()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Genre 1" },
                new Genre { GenreId = 2, Name = "Genre 2" }
            };

            _genreRepositoryMock.Setup(repo => repo.GetAllGenresAsync()).ReturnsAsync(genres);

            var expectedGenreDTOs = new List<GenreDTO>();

            _mapperMock.Setup(mapper => mapper.Map<ICollection<GenreDTO>>(genres)).Returns(expectedGenreDTOs);

            // Act
            var result = await _genreManagementService.GetAllGenresAsync();

            // Assert
            _genreRepositoryMock.Verify(repo => repo.GetAllGenresAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<GenreDTO>>(genres), Times.Once);

            Assert.Same(expectedGenreDTOs, result);
        }

        [Fact]
        public async Task GetBooksByGenreAsync_WithValidGenreId_ShouldReturnBooks()
        {
            // Arrange
            var genreId = 1;
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1" },
                new Book { BookId = 2, Title = "Book 2" }
            };

            _genreRepositoryMock.Setup(repo => repo.GetBooksByGenreAsync(genreId)).ReturnsAsync(books);

            var expectedBookDTOs = new List<BookDTO>();

            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookDTO>>(books)).Returns(expectedBookDTOs);

            // Act
            var result = await _genreManagementService.GetBooksByGenreAsync(genreId);

            // Assert
            _genreRepositoryMock.Verify(repo => repo.GetBooksByGenreAsync(genreId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookDTO>>(books), Times.Once);

            Assert.Same(expectedBookDTOs, result);
        }
    }
}
