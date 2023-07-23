using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace Libro.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IBookManagementService> _bookServiceMock;
        private readonly Mock<IUserManagementService> _userServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _bookServiceMock = new Mock<IBookManagementService>();
            _userServiceMock = new Mock<IUserManagementService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _controller = new HomeController(_notificationServiceMock.Object, _userServiceMock.Object, _bookServiceMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithAvailableBooks()
        {
            // Arrange
            int userId = 1;
            var availableBooks = new List<BookDTO>
            {
                new BookDTO { Title = "Book 1", AvailabilityStatus = AvailabilityStatus.Available },
                new BookDTO { Title = "Book 2", AvailabilityStatus = AvailabilityStatus.Available }
            };
            var bookRecommendations = new List<BookDTO>
            {
                new BookDTO { Title = "Book 1", AvailabilityStatus = AvailabilityStatus.Available }
            };
            var notifications = new List<NotificationDTO>
            {
                new NotificationDTO { Id = 1, UserId = userId }
            };

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };
            _bookServiceMock.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(availableBooks);
            _userServiceMock.Setup(x => x.GetUserRecommendationsAsync(userId)).ReturnsAsync(bookRecommendations);
            _notificationServiceMock.Setup(x => x.GetNotificationsForUserAsync(userId)).ReturnsAsync(notifications);

            // Act
            var result = await _controller.IndexAsync();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(viewResult.Model);

            Assert.Equal(availableBooks, model.AvailableBooks);
        }
    }
}
