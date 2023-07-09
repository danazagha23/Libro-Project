using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IUserManagementService> _userManagementServiceMock;
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _validationServiceMock = new Mock<IValidationService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new AdminController(
                _userManagementServiceMock.Object,
                _validationServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task AssignRole_ValidModel_RedirectToView()
        {
            // Arrange
            var assignRoleViewModel = new AssignRoleViewModel
            {
                Username = "testuser",
                Role = UserRole.Librarian
            };

            // Configure the UserManagementService mock to return a successful role assignment
            _userManagementServiceMock
                .Setup(x => x.AssignRoleAsync(It.IsAny<string>(), It.IsAny<UserRole>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AssignRole(assignRoleViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assignRoleViewModel, result.Model);
        }

        [Fact]
        public async Task AssignRole_InvalidModel_RedirectToView()
        {
            // Arrange
            var assignRoleViewModel = new AssignRoleViewModel();

            // Act
            var result = await _controller.AssignRole(assignRoleViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assignRoleViewModel, result.Model);
        }

        [Fact]
        public async Task Patrons_ReturnsViewWithViewModel()
        {
            // Arrange
            var users = new List<UserDTO>
            {
                new UserDTO { Role = UserRole.Patron },
                new UserDTO { Role = UserRole.Patron }
            };

            _userManagementServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.Patrons(null, 1, 5) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<UsersViewModel>(result.Model);

            Assert.Equal(users, model.Users);
            Assert.Equal(users, model.FilteredUsers);
            Assert.Null(model.SelectedUser);
            Assert.Equal(1, model.PageNumber);
            Assert.Equal(1, model.TotalPages);
        }

        [Fact]
        public async Task EditUser_ValidModel_RedirectToAction()
        {
            // Arrange
            var userDTO = new UserDTO { UserId = 1 };
            var editUserViewModel = new EditUserViewModel { UserId = 1 };

            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<EditUserViewModel>())).Returns(new UserDTO());
            _userManagementServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(userDTO);
            _userManagementServiceMock.Setup(x => x.UpdateUserAsync(1, It.IsAny<UserDTO>())).ReturnsAsync(new UserDTO());

            // Act
            var result = await _controller.EditUser(editUserViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"{userDTO.Role}s", result.ActionName);
        }

        [Fact]
        public async Task EditUser_InvalidModel_ReturnsView()
        {
            // Arrange
            var editUserViewModel = new EditUserViewModel();
            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<RegisterViewModel>())).Returns(new UserDTO());

            // Act
            var result = await _controller.EditUser(editUserViewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(editUserViewModel, result.Model);
        }

        [Fact]
        public async Task Librarians_ReturnsViewWithViewModel()
        {
            // Arrange
            var users = new List<UserDTO>
            {
                new UserDTO { Role = UserRole.Librarian },
                new UserDTO { Role = UserRole.Librarian }
            };

            _userManagementServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.Librarians(null, 1, 5) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<UsersViewModel>(result.Model);

            Assert.Equal(users, model.Users);
            Assert.Equal(users, model.FilteredUsers);
            Assert.Null(model.SelectedUser);
            Assert.Equal(1, model.PageNumber);
            Assert.Equal(1, model.TotalPages);
        }   

        [Fact]
        public void CreateUser_ReturnsView()
            {
                // Act
                var result = _controller.CreateUser() as ViewResult;

                // Assert
                Assert.NotNull(result);
            }

            [Fact]
            public async Task CreateUser_ValidModel_RedirectToAction()
            {
                // Arrange
                var registerViewModel = new RegisterViewModel
                {
                    Username = "testuser",
                    Password = "testpassword",
                    ConfirmPassword = "testpassword",
                    Email = "testuser@example.com",
                    FirstName = "test",
                    LastName = "user",
                    PhoneNumber = "0595409501",
                    Address = "Nablus"
                };

                _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<RegisterViewModel>())).Returns(new UserDTO());
                _userManagementServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(new UserDTO());

                // Act
                var result = await _controller.CreateUser(registerViewModel) as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Librarians", result.ActionName);
            }
        }
    }
