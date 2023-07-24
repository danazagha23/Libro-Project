using Libro.Domain.Entities;
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
    public class AuthorRepositoryTests
    {
        private AuthorRepository _authorRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<AuthorRepository>> _logger;

        public AuthorRepositoryTests()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<AuthorRepository>>();

            _authorRepository = new AuthorRepository(_dbContext, _logger.Object);
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ShouldReturnAllAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new Author { AuthorId = 1, AuthorName = "Author 1" },
                new Author { AuthorId = 2, AuthorName = "Author 2" },
                new Author { AuthorId = 3, AuthorName = "Author 3" }
            };

            _dbContext.Authors.AddRange(authors);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _authorRepository.GetAllAuthorsAsync();

            // Assert
            Assert.Equal(authors.Count, result.Count);
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ShouldReturnAuthorWhenExists()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, AuthorName = "Author 1" };

            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _authorRepository.GetAuthorByIdAsync(authorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorId, result.AuthorId);

            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ShouldThrowExceptionWhenAuthorNotExists()
        {
            // Arrange
            var nonExistentAuthorId = 100;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authorRepository.GetAuthorByIdAsync(nonExistentAuthorId));
            
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAuthorByNameAsync_ShouldReturnAuthorWhenExists()
        {
            // Arrange
            var authorName = "Author 1";
            var author = new Author { AuthorId = 1, AuthorName = authorName };

            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _authorRepository.GetAuthorByNameAsync(authorName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorName, result.AuthorName);

            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAuthorByNameAsync_ShouldThrowExceptionWhenAuthorNotExists()
        {
            // Arrange
            var nonExistentAuthorName = "NonExistentAuthor";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authorRepository.GetAuthorByNameAsync(nonExistentAuthorName));

            _dbContext.Dispose();
        }

        [Fact]
        public async Task CreateAuthorAsync_ShouldCreateNewAuthor()
        {
            // Arrange
            var author = new Author { AuthorName = "New Author" };

            // Act
            var result = await _authorRepository.CreateAuthorAsync(author);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.AuthorId > 0);

            // Check if the author is added to the database
            var createdAuthor = await _dbContext.Authors.FindAsync(result.AuthorId);
            Assert.NotNull(createdAuthor);
            Assert.Equal(author.AuthorName, createdAuthor.AuthorName);

            _dbContext.Dispose();
        }

        [Fact]
        public async Task UpdateAuthorAsync_ShouldUpdateExistingAuthor()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, AuthorName = "Author 1" };
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();

            // Update the author name
            var updatedAuthorName = "Updated Author";
            var updatedAuthor = new Author { AuthorId = authorId, AuthorName = updatedAuthorName };

            // Act
            var result = await _authorRepository.UpdateAuthorAsync(authorId, updatedAuthor);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedAuthorName, result.AuthorName);

            // Check if the author is updated in the database
            var updatedAuthorInDb = await _dbContext.Authors.FindAsync(authorId);
            Assert.NotNull(updatedAuthorInDb);
            Assert.Equal(updatedAuthorName, updatedAuthorInDb.AuthorName);

            _dbContext.Dispose();
        }

        [Fact]
        public async Task UpdateAuthorAsync_ShouldThrowExceptionWhenAuthorNotExists()
        {
            // Arrange
            var nonExistentAuthorId = 100;
            var author = new Author { AuthorId = nonExistentAuthorId, AuthorName = "NonExistentAuthor" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authorRepository.UpdateAuthorAsync(nonExistentAuthorId, author));

            _dbContext.Dispose();
        }

        [Fact]
        public async Task DeleteAuthorAsync_ShouldDeleteExistingAuthor()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId, AuthorName = "Author 1" };
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _authorRepository.DeleteAuthorAsync(authorId);

            // Assert
            Assert.True(result);

            // Check if the author is deleted from the database
            var deletedAuthorInDb = await _dbContext.Authors.FindAsync(authorId);
            Assert.Null(deletedAuthorInDb);

            _dbContext.Dispose();
        }

        [Fact]
        public async Task DeleteAuthorAsync_ShouldThrowExceptionWhenAuthorNotExists()
        {
            // Arrange
            var nonExistentAuthorId = 100;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authorRepository.DeleteAuthorAsync(nonExistentAuthorId));

            _dbContext.Dispose();
        }
    }
}
