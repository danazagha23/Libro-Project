using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IBookManagementService> _bookServiceMock;
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _bookServiceMock = new Mock<IBookManagementService>();
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object, _bookServiceMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithAvailableBooks()
        {
            // Arrange
            var availableBooks = new List<BookDTO>
            {
                new BookDTO { Title = "Book 1", AvailabilityStatus = AvailabilityStatus.Available },
                new BookDTO { Title = "Book 2", AvailabilityStatus = AvailabilityStatus.Available }
            };

            _bookServiceMock.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(availableBooks);

            // Act
            var result = await _controller.IndexAsync();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(viewResult.Model);

            Assert.Equal(availableBooks, model.AvailableBooks);
        }
    }
}
