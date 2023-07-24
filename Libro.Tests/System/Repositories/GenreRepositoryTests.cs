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
    public class GenreRepositoryTests
    {
        private GenreRepository _genreRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<GenreRepository>> _logger;

        public GenreRepositoryTests()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<GenreRepository>>();

            _genreRepository = new GenreRepository(_dbContext, _logger.Object);
        }

        [Fact]
        public async Task GetAllGenresAsync_ShouldReturnAllGenres()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Genre 1" },
                new Genre { GenreId = 2, Name = "Genre 2" },
                new Genre { GenreId = 3, Name = "Genre 3" }
            };

            _dbContext.Genres.AddRange(genres);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _genreRepository.GetAllGenresAsync();

            // Assert
            Assert.Equal(genres.Count, result.Count);
        }

        [Fact]
        public async Task GetBooksByGenreAsync_ShouldReturnBooksByGenreId()
        {
            // Arrange
            var genreId = 1;
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1", Description = "description", GenreId = genreId },
                new Book { BookId = 2, Title = "Book 2", Description = "description", GenreId = genreId },
                new Book { BookId = 3, Title = "Book 3", Description = "description", GenreId = genreId }
            };

            _dbContext.Books.AddRange(books);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _genreRepository.GetBooksByGenreAsync(genreId);

            // Assert
            Assert.Equal(books.Count, result.Count);
        }
    }
}
