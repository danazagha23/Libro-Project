using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace Libro.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IUserManagementService> _userManagementServiceMock;
        private readonly Mock<IReadingListService> _readingListServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<Application.ServicesInterfaces.IAuthenticationService> _authenticationServiceMock;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _readingListServiceMock = new Mock<IReadingListService>();
            _authenticationServiceMock = new Mock<Application.ServicesInterfaces.IAuthenticationService>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _accountController = new AccountController(
                _userManagementServiceMock.Object,
                _readingListServiceMock.Object,
                _mapperMock.Object,
                _authenticationServiceMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public void Register_GET_ReturnsView()
        {
            // Act
            var result = _accountController.Register();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Register_POST_ValidModel_RedirectsToLogin()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Username = "user1",
                Password = "12345678",
                Email = "user@gmail.com",
                FirstName = "user",
                LastName = "user",
                PhoneNumber = "0595409501",
                Address = "nablus"
            };

            var userDTO = new UserDTO
            {
                UserId = 1,
                Username = "user1",
                Password = "12345678",
                Email = "user@gmail.com",
                FirstName = "user",
                LastName = "user",
                PhoneNumber = "0595409501",
                Address = "nablus",
                Role = UserRole.Patron
            };

            _mapperMock.Setup(m => m.Map<UserDTO>(registerViewModel)).Returns(userDTO);
            _userManagementServiceMock.Setup(u => u.CreateUserAsync(userDTO)).Verifiable();

            // Act
            var result = await _accountController.Register(registerViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
            Assert.Null(redirectToActionResult.ControllerName);

            _userManagementServiceMock.Verify();
        }

        [Fact]
        public async Task Register_POST_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Username = "user1"
            };

            _accountController.ModelState.AddModelError("", "Some error message");

            // Act
            var result = await _accountController.Register(registerViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(registerViewModel, viewResult.Model);
        }

        [Fact]
        public void Login_GET_ReturnsView()
        {
            // Act
            var result = _accountController.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Login_POST_ValidModel_RedirectsToHomeIndex()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Username = "user1",
                Password = "12345678"
            };

            var userDTO = new UserDTO
            {
                UserId = 1,
                Username = "user1",
                Role = UserRole.Patron
            };

            _userManagementServiceMock.Setup(u => u.AuthenticateUserAsync(loginViewModel.Username, loginViewModel.Password)).ReturnsAsync(userDTO);
            _configurationMock.Setup(c => c["JwtSettings:SecretKey"]).Returns("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            _configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns("http://localhost:5228/");
            _configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns("http://localhost:5228");
            
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            _accountController.ControllerContext = controllerContext;

            // Act
            var result = await _accountController.Login(loginViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }


        [Fact]
        public async Task Login_POST_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var loginViewModel = new LoginViewModel 
            {
                Username = "dana"
            };

            _accountController.ModelState.AddModelError("error", "Some error message");

            // Act
            var result = await _accountController.Login(loginViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(loginViewModel, viewResult.Model);
        }

        [Fact]
        public async Task ProfileAsync_GET_AuthorizedUser_ReturnsViewWithModel()
        {
            // Arrange
            var userId = 1;
            var userDTO = new UserDTO { UserId = userId };

            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, CookieAuthenticationDefaults.AuthenticationScheme));

            _userManagementServiceMock.Setup(u => u.GetUserByIdAsync(userId)).ReturnsAsync(userDTO);
            _userManagementServiceMock.Setup(u => u.GetBorrowingHistoryAsync(userId)).ReturnsAsync(new List<BookTransactionDTO>());
            _userManagementServiceMock.Setup(u => u.GetCurrentLoansAsync(userId)).ReturnsAsync(new List<BookTransactionDTO>());
            _userManagementServiceMock.Setup(u => u.GetOverdueLoansAsync(userId)).ReturnsAsync(new List<BookTransactionDTO>());
            _readingListServiceMock.Setup(r => r.GetReadingListsByUserIdAsync(userId)).ReturnsAsync(new List<ReadingListDTO>());

            _accountController.ControllerContext.HttpContext = context;

            // Act
            var result = await _accountController.ProfileAsync();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var userProfile = Assert.IsType<UserProfileViewModel>(viewResult.Model);
            Assert.Equal(userDTO, userProfile.User);

            _userManagementServiceMock.Verify();
            _readingListServiceMock.Verify();
        }
    }
}
