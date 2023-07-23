using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Presentation.Controllers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IUserManagementService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _userServiceMock = new Mock<IUserManagementService>();
            _mapperMock = new Mock<IMapper>();
            //_controller = new AccountController(_userServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Register_ValidModel_RedirectToLogin()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Username = "dana_dana",
                Password = "danadana12",
                ConfirmPassword = "danadana12",
                Email = "danaz@gmail.com",
                FirstName = "dana",
                LastName = "dana",
                PhoneNumber = "0595409501",
                Address = "Nablus"
            };

            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<RegisterViewModel>())).Returns(new UserDTO());
            _userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(new UserDTO());

            // Act
            var result = await _controller.Register(registerViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewResultWithModel()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Username = "dana_dana",
                Password = "missmach",
                ConfirmPassword = "wrongmatch",
                Email = "wrongEmail",
                FirstName = "dana",
                LastName = "dana",
                PhoneNumber = "0595409501",
                Address = "Nablus"
            };

            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<RegisterViewModel>())).Returns(new UserDTO());
            _userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(new UserDTO());

            // Act
            var result = await _controller.Register(registerViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(registerViewModel, result.Model);
            Assert.Equal("", result.ViewName);
        }

        [Fact]
        public async Task Login_ValidCredentials_RedirectToDashboard()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Username = "admin",
                Password = "adminadmin"
            };

            _userServiceMock.Setup(x => x.AuthenticateUserAsync(loginViewModel.Username, loginViewModel.Password))
                .ReturnsAsync(new UserDTO { Username = loginViewModel.Username });

            // Act
            var result = await _controller.Login(loginViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewResultWithModel()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Username = "testuser",
                Password = ""
            };

            // Act
            var result = await _controller.Login(loginViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(loginViewModel, result.Model);
        }

        [Fact]
        public async Task Logout_RedirectToHomeIndex()
        {
            // Act
            var result = await _controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }
    }
}
