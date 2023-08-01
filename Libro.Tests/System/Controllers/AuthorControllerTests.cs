using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
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
    public class AuthorsControllerTests
    {
        private readonly Mock<IAuthorManagementService> _authorServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPaginationWrapper<AuthorDTO>> _paginationWrapper;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _authorServiceMock = new Mock<IAuthorManagementService>();
            _mapperMock = new Mock<IMapper>();
            _paginationWrapper = new Mock<IPaginationWrapper<AuthorDTO>>();
            _controller = new AuthorsController
                (_authorServiceMock.Object,
                _mapperMock.Object,
                _paginationWrapper.Object
                );
        }

        [Fact]
        public async Task Index_ReturnsViewResultWithAuthorsViewModel()
        {
            // Arrange
            var authors = new List<AuthorDTO>
            {
                new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" },
                new AuthorDTO { AuthorId = 2, AuthorName = "Author 2" },
                new AuthorDTO { AuthorId = 3, AuthorName = "Author 3" }
            };

            _authorServiceMock.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(authors);
            _paginationWrapper.Setup(x => x.GetPage(authors, 1, 5)).Returns(authors);
            _paginationWrapper.Setup(x => x.GetTotalPages(authors, 5)).Returns(1);

            var authorsViewModel = new AuthorsViewModel
            {
                Authors = authors,
                FilteredAuthors = authors,
                SelectedAuthor = null,
                PageNumber = 1,
                TotalPages = 1
            };

            // Act
            var result = await _controller.Index(null) as ViewResult;
            var model = result?.Model as AuthorsViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorsViewModel.Authors.Count, model?.Authors.Count);
            Assert.Equal(authorsViewModel.FilteredAuthors.Count, model?.FilteredAuthors.Count);
            Assert.Equal(authorsViewModel.SelectedAuthor, model?.SelectedAuthor);
            Assert.Equal(authorsViewModel.PageNumber, model?.PageNumber);
            Assert.Equal(authorsViewModel.TotalPages, model?.TotalPages);
        }

        [Fact]
        public async Task EditAuthor_ExistingAuthorId_ReturnsViewResultWithAuthorViewModel()
        {
            // Arrange
            var authorId = 1;
            var author = new AuthorDTO { AuthorId = authorId, AuthorName = "Author 1" };
            var authorViewModel = new AuthorViewModel { AuthorId = authorId, AuthorName = "Author 1" };

            _authorServiceMock.Setup(x => x.GetAuthorByIdAsync(authorId)).ReturnsAsync(author);
            _mapperMock.Setup(x => x.Map<AuthorViewModel>(author)).Returns(authorViewModel);

            // Act
            var result = await _controller.EditAuthor(authorId) as ViewResult;
            var model = result?.Model as AuthorViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorViewModel.AuthorId, model?.AuthorId);
            Assert.Equal(authorViewModel.AuthorName, model?.AuthorName);
        }

        [Fact]
        public async Task EditAuthor_NonExistingAuthorId_ReturnsNotFound()
        {
            // Arrange
            var authorId = 1;
            AuthorDTO author = null;

            _authorServiceMock.Setup(x => x.GetAuthorByIdAsync(authorId)).ReturnsAsync(author);

            // Act
            var result = await _controller.EditAuthor(authorId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAuthor_ValidAuthorId_RedirectToIndex()
        {
            // Arrange
            var authorId = 1;

            _authorServiceMock.Setup(x => x.DeleteAuthorAsync(authorId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAuthor(authorId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task DeleteAuthor_InvalidAuthorId_ReturnsNotFound()
        {
            // Arrange
            var authorId = 1;

            _authorServiceMock.Setup(x => x.DeleteAuthorAsync(authorId)).Throws(new Exception("Author not Found"));

            // Act
            var result = await _controller.DeleteAuthor(authorId);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreateAuthor_ValidModel_RedirectToIndex()
        {
            // Arrange
            var authorViewModel = new AuthorViewModel
            {
                AuthorId = 1,
                AuthorName = "Test Author"
            };

            var authorDTO = new AuthorDTO
            {
                AuthorId = 1,
                AuthorName = "Test Author"
            };

            _mapperMock.Setup(x => x.Map<AuthorDTO>(authorViewModel)).Returns(authorDTO);
            _authorServiceMock.Setup(x => x.CreateAuthorAsync(authorDTO)).ReturnsAsync(new AuthorDTO());

            // Act
            var result = await _controller.CreateAuthor(authorViewModel) as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.Url);
        }
    }
}
