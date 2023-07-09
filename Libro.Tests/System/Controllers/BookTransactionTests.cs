using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Controllers
{
    public class BookTransactionsControllerTests
    {
        private readonly Mock<IBookTransactionsService> _bookTransactionsServiceMock;
        private readonly Mock<IBookManagementService> _bookManagementServiceMock;
        private readonly Mock<IUserManagementService> _userManagementServiceMock;
        private readonly BookTransactionsController _controller;

        public BookTransactionsControllerTests()
        {
            _bookTransactionsServiceMock = new Mock<IBookTransactionsService>();
            _bookManagementServiceMock = new Mock<IBookManagementService>();
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _controller = new BookTransactionsController(_bookTransactionsServiceMock.Object, _bookManagementServiceMock.Object, _userManagementServiceMock.Object);
        }

        [Fact]
        public async Task Reserve_ValidBookIdAndPatronId_RedirectsToBookDetailsWithSuccessMessage()
        {
            // Arrange
            var bookId = 1;
            var patronId = 1;
            var userId = "1";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var claimsIdentity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _bookTransactionsServiceMock.Setup(x => x.ReserveBookAsync(bookId, patronId)).Verifiable();

            // Act
            var result = await _controller.Reserve(bookId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Books", result.ControllerName);
            Assert.Equal(bookId, result.RouteValues["id"]);
        }

        [Fact]
        public async Task Reserve_InvalidUserId_ReturnsBadRequestWithErrorMessage()
        {
            // Arrange
            var bookId = 1;
            string userId = null;

            var claims = new List<Claim>();
            var claimsIdentity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = await _controller.Reserve(bookId) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User ID not found", result.Value);
        }

        //[Fact]
        //public async Task Index_ReturnsViewResultWithTransactionsViewModel()
        //{
        //    // Arrange
        //    var selectedType = "Type1";
        //    var selectedPatron = "Patron1";
        //    var selectedBook = "Book1";
        //    var status = "Status1";
        //    var page = 1;
        //    var pageSize = 5;

        //    var bookTransactions = new List<BookTransactionDTO>
        //    {
        //        new BookTransactionDTO { TransactionId = 1, PatronId = 1, BookId = 1, },
        //        new BookTransactionDTO { TransactionId = 2, PatronId = 2, Book = 2, Status = "Status2" }
        //    };

        //    var searchResults = new List<BookTransactionDTO>
        //    {
        //        new BookTransactionDTO { TransactionId = 1, Type = "Type1", Patron = "Patron1", Book = "Book1", Status = "Status1" }
        //    };

        //    var users = new List<UserDTO>
        //    {
        //        new UserDTO { Role = UserRole.Patron }
        //    };

        //    var patrons = new List<UserDTO>
        //    {
        //        new UserDTO { Role = UserRole.Patron }
        //    };

        //    var books = new List<BookDTO>
        //    {
        //        new BookDTO { Title = "Book1" },
        //        new BookDTO { Title = "Book2" }
        //    };

        //    _bookTransactionsServiceMock.Setup(x => x.GetAllBookTransactionsAsync()).ReturnsAsync(bookTransactions);
        //    _bookTransactionsServiceMock.Setup(x => x.FindTransactionsAsync(selectedType, selectedPatron, selectedBook, status)).ReturnsAsync(searchResults);
        //    _userManagementServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);
        //    _bookManagementServiceMock.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(books);

        //    var transactionsViewModel = new BookTransactionsViewModel
        //    {
        //        Transactions = bookTransactions,
        //        FilteredTransactions = searchResults,
        //        Books = books,
        //        Patrons = patrons,
        //        SelectedBook = selectedBook,
        //        SelectedPatron = selectedPatron,
        //        SelectedType = selectedType,
        //        PageNumber = page,
        //        TotalPages = 1
        //    };

        //    // Act
        //    var result = await _controller.Index(selectedType, selectedPatron, selectedBook, status, page, pageSize) as ViewResult;
        //    var model = result?.Model as BookTransactionsViewModel;

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.NotNull(model);
        //    Assert.Equal(transactionsViewModel.Transactions.Count, model.Transactions.Count);
        //    Assert.Equal(transactionsViewModel.FilteredTransactions.Count, model.FilteredTransactions.Count);
        //    Assert.Equal(transactionsViewModel.Books.Count, model.Books.Count);
        //    Assert.Equal(transactionsViewModel.Patrons.Count, model.Patrons.Count);
        //    Assert.Equal(transactionsViewModel.SelectedBook, model.SelectedBook);
        //    Assert.Equal(transactionsViewModel.SelectedPatron, model.SelectedPatron);
        //    Assert.Equal(transactionsViewModel.SelectedType, model.SelectedType);
        //    Assert.Equal(transactionsViewModel.PageNumber, model.PageNumber);
        //    Assert.Equal(transactionsViewModel.TotalPages, model.TotalPages);
        //}

        [Fact]
        public async Task CheckOut_ValidTransactionId_RedirectsToIndex()
        {
            // Arrange
            var transactionId = 1;

            _bookTransactionsServiceMock.Setup(x => x.CheckOutBookAsync(transactionId)).Verifiable();

            // Act
            var result = await _controller.CheckOut(transactionId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AcceptReturn_ValidTransactionId_RedirectsToIndex()
        {
            // Arrange
            var transactionId = 1;

            _bookTransactionsServiceMock.Setup(x => x.AcceptReturnAsync(transactionId)).Verifiable();

            // Act
            var result = await _controller.AcceptReturn(transactionId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }
    }
}
