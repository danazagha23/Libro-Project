using Auth0.ManagementApi.Models;
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
using User = Libro.Domain.Entities.User;

namespace Libro.Tests.System.Repositories
{
    public class ReviewRepositoryTests
    {
        private ReviewRepository _reviewRepository;
        private LibroDbContext _dbContext;
        private Mock<ILogger<ReviewRepository>> _logger;

        public ReviewRepositoryTests()
        {
            // Create a new in-memory database with a unique name for each test case
            var options = new DbContextOptionsBuilder<LibroDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _dbContext = new LibroDbContext(options);

            // Initialize the logger mock
            _logger = new Mock<ILogger<ReviewRepository>>();

            _reviewRepository = new ReviewRepository(_dbContext, _logger.Object);
        }

        [Fact]
        public async Task GetReviewsByBookIdAsync_ShouldReturnReviewsForBook()
        {
            // Arrange
            var bookId = 1;
            var user = new User { UserId = 1, Username = "user1", Email = "user@gmail.com", FirstName = "user", LastName = "user", Password = "12345678" };
            var book = new Book { BookId = 1, Title = "Book 1", Description = "description..." };

            var reviews = new List<Review>
            {
                new Review { Id = 1, BookId = bookId, Rating = 4, Comment = "Good book", UserId = 1, User = user, Book = book },
                new Review { Id = 2, BookId = bookId, Rating = 5, Comment = "Excellent book", UserId = 1, User = user, Book = book }
            };

            _dbContext.Reviews.AddRange(reviews);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _reviewRepository.GetReviewsByBookIdAsync(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviews.Count, result.Count);
            Assert.Equal(reviews.Select(r => r.Rating), result.Select(r => r.Rating));
        }

        [Fact]
        public async Task GetAverageRatingForBookAsync_ShouldReturnAverageRatingForBook()
        {
            // Arrange
            var bookId = 1;
            var reviews = new List<Review>
            {
                new Review { Id = 1, BookId = bookId, Rating = 4, Comment = "Good book" },
                new Review { Id = 2, BookId = bookId, Rating = 5, Comment = "Excellent book" }
            };

            _dbContext.Reviews.AddRange(reviews);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _reviewRepository.GetAverageRatingForBookAsync(bookId);

            // Assert
            Assert.Equal(4.5, result); // The average rating for the reviews above should be 4.5
        }

        [Fact]
        public async Task CreateReviewAsync_ShouldCreateNewReview()
        {
            // Arrange
            var review = new Review
            {
                BookId = 1,
                Rating = 4,
                Comment = "Good book"
            };

            // Act
            await _reviewRepository.CreateReviewAsync(review);

            // Assert
            var createdReview = await _dbContext.Reviews.FindAsync(review.Id);
            Assert.NotNull(createdReview);
            Assert.Equal(review.Rating, createdReview.Rating);
            Assert.Equal(review.Comment, createdReview.Comment);
        }
    }
}
