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
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _mapperMock = new Mock<IMapper>();
            _reviewService = new ReviewService(_reviewRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetReviewsByBookIdAsync_WithValidBookId_ShouldReturnReviews()
        {
            // Arrange
            var bookId = 1;
            var reviews = new List<Review>
            {
                new Review { Id = 1, BookId = bookId },
                new Review { Id = 2, BookId = bookId }
            };
            var reviewDTOs = new List<ReviewDTO>
            {
                new ReviewDTO { Id = 1, BookId = bookId },
                new ReviewDTO { Id = 2, BookId = bookId }
            };

            _reviewRepositoryMock.Setup(repo => repo.GetReviewsByBookIdAsync(bookId)).ReturnsAsync(reviews);
            _mapperMock.Setup(mapper => mapper.Map<ICollection<ReviewDTO>>(reviews)).Returns(reviewDTOs);

            // Act
            var result = await _reviewService.GetReviewsByBookIdAsync(bookId);

            // Assert
            Assert.Equal(reviewDTOs, result);

            _reviewRepositoryMock.Verify(repo => repo.GetReviewsByBookIdAsync(bookId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<ReviewDTO>>(reviews), Times.Once);
        }

        [Fact]
        public async Task GetAverageRatingForBookAsync_WithValidBookId_ShouldReturnAverageRating()
        {
            // Arrange
            var bookId = 1;
            var averageRating = 4.5;

            _reviewRepositoryMock.Setup(repo => repo.GetAverageRatingForBookAsync(bookId)).ReturnsAsync(averageRating);

            // Act
            var result = await _reviewService.GetAverageRatingForBookAsync(bookId);

            // Assert
            Assert.Equal(averageRating, result);

            _reviewRepositoryMock.Verify(repo => repo.GetAverageRatingForBookAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_WithValidReviewDTO_ShouldCreateReview()
        {
            // Arrange
            var reviewDTO = new ReviewDTO { Id = 1, BookId = 1, Rating = 3, Comment = "Great book" };
            var review = new Review { Id = 1, BookId = 1, Rating = 3, Comment = "Great book" };

            _mapperMock.Setup(mapper => mapper.Map<Review>(reviewDTO)).Returns(review);
            _reviewRepositoryMock.Setup(repo => repo.CreateReviewAsync(review)).Returns(Task.CompletedTask);

            // Act
            await _reviewService.CreateReviewAsync(reviewDTO);

            // Assert
            _mapperMock.Verify(mapper => mapper.Map<Review>(reviewDTO), Times.Once);
            _reviewRepositoryMock.Verify(repo => repo.CreateReviewAsync(review), Times.Once);
        }
    }
}
