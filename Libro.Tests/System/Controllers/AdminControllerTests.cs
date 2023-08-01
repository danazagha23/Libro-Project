using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Helpers;
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
        private readonly Mock<IPaginationWrapper<UserDTO>> _paginationWrapper;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _validationServiceMock = new Mock<IValidationService>();
            _mapperMock = new Mock<IMapper>();
            _paginationWrapper = new Mock<IPaginationWrapper<UserDTO>>();
            _controller = new AdminController(
                _userManagementServiceMock.Object,
                _validationServiceMock.Object,
                _mapperMock.Object,
                _paginationWrapper.Object
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
            var Patrons = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Username = "user1", Role = UserRole.Patron },
                new UserDTO { UserId = 2, Username = "user2", Role = UserRole.Patron },
                new UserDTO { UserId = 3, Username = "user3", Role = UserRole.Patron },
                new UserDTO { UserId = 4, Username = "user4", Role = UserRole.Patron },
                new UserDTO { UserId = 5, Username = "user5", Role = UserRole.Patron },
                new UserDTO { UserId = 6, Username = "user6", Role = UserRole.Patron },
                new UserDTO { UserId = 7, Username = "dana", Role = UserRole.Patron }
            };

            var filteredUsers = Patrons.Where(user => user.Username.Contains("user")).ToList();

            var firstPageUsers = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Username = "user1", Role = UserRole.Patron },
                new UserDTO { UserId = 2, Username = "user2", Role = UserRole.Patron },
                new UserDTO { UserId = 3, Username = "user3", Role = UserRole.Patron },
                new UserDTO { UserId = 4, Username = "user4", Role = UserRole.Patron },
                new UserDTO { UserId = 5, Username = "user5", Role = UserRole.Patron }
            };
            var secondPageUsers = new List<UserDTO>
            {
                new UserDTO { UserId = 6, Username = "user6", Role = UserRole.Patron }
            };

            _userManagementServiceMock.Setup(x => x.GetUsersByRoleAsync(UserRole.Patron)).ReturnsAsync(Patrons);
            _paginationWrapper.Setup(x => x.GetPage(filteredUsers, 1, 5)).Returns(firstPageUsers);
            _paginationWrapper.Setup(x => x.GetPage(filteredUsers, 2, 5)).Returns(secondPageUsers);
            _paginationWrapper.Setup(x => x.GetTotalPages(filteredUsers, 5)).Returns(2);

            // Act
            var result = await _controller.Patrons("user", 1, 5) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<UsersViewModel>(result.Model);

            Assert.Equal(Patrons, model.Users);
            Assert.Equal(firstPageUsers, model.FilteredUsers);
            Assert.Equal(1, model.PageNumber);
            Assert.Equal(2, model.TotalPages);
        }

        [Fact]
        public async Task EditUser_ValidModel_UpdateUsername()
        {
            // Arrange
            var userDTO = new UserDTO 
            { 
                UserId = 1,
                Username = "user1",
                FirstName = "user",
                LastName = "1",
                Password = "12345678",
                Email = "user@gmail.com",
                Address = "Nablus",
                 PhoneNumber = "0595409501",
                Role = UserRole.Patron      
            };
            var editUserViewModel = new EditUserViewModel
            {
                UserId = 1,
                Username = "updated",
                FirstName = "user",
                LastName = "1",
                Email = "user@gmail.com",
                Address = "Nablus",
                PhoneNumber = "0595409501"
            };
            var UpdatedUserDTO = new UserDTO
            {
                UserId = 1,
                Username = "updated",
                FirstName = "user",
                LastName = "1",
                Password = "12345678",
                Email = "user@gmail.com",
                Address = "Nablus",
                PhoneNumber = "0595409501",
                Role = UserRole.Patron
            };

            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<EditUserViewModel>())).Returns(userDTO);
            _userManagementServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(userDTO);
            _userManagementServiceMock.Setup(x => x.UpdateUserAsync(1, userDTO)).ReturnsAsync(UpdatedUserDTO);

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
            var userDTO = new UserDTO
            {
                UserId = 1,
                Username = "user1",
                FirstName = "user",
                LastName = "1",
                Password = "12345678",
                Email = "user@gmail.com",
                Address = "Nablus",
                PhoneNumber = "0595409501",
                Role = UserRole.Patron
            };
            var editUserViewModel = new EditUserViewModel();
            var UpdatedUserDTO = new UserDTO();

            _mapperMock.Setup(x => x.Map<UserDTO>(It.IsAny<EditUserViewModel>())).Returns(userDTO);
            _userManagementServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(userDTO);
            _userManagementServiceMock.Setup(x => x.UpdateUserAsync(1, userDTO)).ReturnsAsync(UpdatedUserDTO);

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

            _userManagementServiceMock.Setup(x => x.GetUsersByRoleAsync(UserRole.Librarian)).ReturnsAsync(users);
            _paginationWrapper.Setup(x => x.GetPage(users, 1, 5)).Returns(users);
            _paginationWrapper.Setup(x => x.GetTotalPages(users, 5)).Returns(1);

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
