using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Helpers;
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
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IPaginationWrapper<BookTransactionDTO>> _paginationWrapper;
        private readonly BookTransactionsController _bookTransactionsController;

        public BookTransactionsControllerTests()
        {
            _bookTransactionsServiceMock = new Mock<IBookTransactionsService>();
            _bookManagementServiceMock = new Mock<IBookManagementService>();
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _paginationWrapper = new Mock<IPaginationWrapper<BookTransactionDTO>>();
            _bookTransactionsController = new BookTransactionsController(
                _bookTransactionsServiceMock.Object,
                _bookManagementServiceMock.Object,
                _userManagementServiceMock.Object,
                _paginationWrapper.Object
            );
        }

        [Fact]
        public async Task Reserve_ValidBookId_ReturnsRedirectToAction()
        {
            // Arrange
            int bookId = 1;
            int patronId = 2;

            var user = new UserDTO { UserId = patronId, Role = UserRole.Patron };
            var book = new BookDTO { BookId = bookId, AvailabilityStatus = AvailabilityStatus.Available };

            _bookTransactionsServiceMock.Setup(s => s.ReserveBookAsync(bookId, patronId)).Verifiable();
            _userManagementServiceMock.Setup(s => s.GetUserByIdAsync(patronId)).ReturnsAsync(user);
            _bookManagementServiceMock.Setup(s => s.GetBookByIdAsync(bookId)).ReturnsAsync(book);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, patronId.ToString())
            };
            _bookTransactionsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            // Act
            var result = await _bookTransactionsController.Reserve(bookId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BookDetails", redirectToActionResult.ActionName);
            Assert.Equal("Books", redirectToActionResult.ControllerName);
            Assert.Equal(bookId, redirectToActionResult.RouteValues["id"]);

            _bookTransactionsServiceMock.Verify();
            _userManagementServiceMock.Verify();
            _bookManagementServiceMock.Verify();
        }

        [Fact]
        public async Task Reserve_InvalidBookId_ReturnsBadRequest()
        {
            // Arrange
            int bookId = 1;

            _bookTransactionsServiceMock.Setup(s => s.ReserveBookAsync(bookId, It.IsAny<int>()))
                .ThrowsAsync(new Exception("Book not found"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "2")
            };
            _bookTransactionsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            // Act
            var result = await _bookTransactionsController.Reserve(bookId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Book not found", badRequestObjectResult.Value);

            _bookTransactionsServiceMock.Verify();
        }

        [Fact]
        public async Task Index_ReturnsViewWithTransactionsViewModel()
        {
            // Arrange
            var transactions = new List<BookTransactionDTO>
            {
                new BookTransactionDTO { TransactionId = 1, TransactionType = TransactionType.Reserved },
                new BookTransactionDTO { TransactionId = 2, TransactionType = TransactionType.Borrowed }
            };
            var filteredTransactions = new List<BookTransactionDTO>
            {
                new BookTransactionDTO { TransactionId = 2, TransactionType = TransactionType.Borrowed }
            };
            var books = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1" },
                new BookDTO { BookId = 2, Title = "Book 2" }
            };
            var users = new List<UserDTO>
            {
                new UserDTO { UserId = 1, Role = UserRole.Patron },
                new UserDTO { UserId = 2, Role = UserRole.Patron }
            };
            var selectedType = "Borrowed";
            var selectedPatron = "1";
            var selectedBook = "1";

            _bookTransactionsServiceMock.Setup(s => s.GetAllBookTransactionsAsync()).ReturnsAsync(transactions);
            _bookTransactionsServiceMock.Setup(s => s.FindTransactionsAsync(selectedType, selectedPatron, selectedBook))
                .ReturnsAsync(filteredTransactions);
            _userManagementServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
            _bookManagementServiceMock.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(books);
            _paginationWrapper.Setup(x => x.GetPage(filteredTransactions, 1, 5)).Returns(filteredTransactions);
            _paginationWrapper.Setup(x => x.GetTotalPages(filteredTransactions, 5)).Returns(1);

            // Act
            var result = await _bookTransactionsController.Index(selectedType, selectedPatron, selectedBook, 1, 5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var transactionsViewModel = Assert.IsType<BookTransactionsViewModel>(viewResult.Model);
            Assert.Equal(transactions.Count(t => t.TransactionType.ToString() == selectedType), transactionsViewModel.FilteredTransactions.Count);
            Assert.Equal(transactions, transactionsViewModel.Transactions);
            Assert.Equal(books, transactionsViewModel.Books);
            Assert.Equal(filteredTransactions, transactionsViewModel.FilteredTransactions);
            Assert.Equal(users, transactionsViewModel.Patrons);
            Assert.Equal(selectedType, transactionsViewModel.SelectedType);
            Assert.Equal(selectedPatron, transactionsViewModel.SelectedPatron);
            Assert.Equal(selectedBook, transactionsViewModel.SelectedBook);
            Assert.Equal(1, transactionsViewModel.PageNumber);

            _bookTransactionsServiceMock.Verify();
            _userManagementServiceMock.Verify();
            _bookManagementServiceMock.Verify();
        }

        [Fact]
        public async Task CheckOut_ValidTransactionId_ReturnsRedirectToAction()
        {
            // Arrange
            int transactionId = 1;

            _bookTransactionsServiceMock.Setup(s => s.CheckOutBookAsync(transactionId)).Verifiable();

            // Act
            var result = await _bookTransactionsController.CheckOut(transactionId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            _bookTransactionsServiceMock.Verify();
        }

        [Fact]
        public async Task CheckOut_InvalidTransactionId_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = 1;

            _bookTransactionsServiceMock.Setup(s => s.CheckOutBookAsync(transactionId))
                .ThrowsAsync(new Exception("Book transaction not found"));

            // Act
            var result = await _bookTransactionsController.CheckOut(transactionId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Book transaction not found", badRequestObjectResult.Value);

            _bookTransactionsServiceMock.Verify();
        }

        [Fact]
        public async Task AcceptReturn_ValidTransactionId_ReturnsRedirectToAction()
        {
            // Arrange
            int transactionId = 1;

            _bookTransactionsServiceMock.Setup(s => s.AcceptReturnAsync(transactionId)).Verifiable();

            // Act
            var result = await _bookTransactionsController.AcceptReturn(transactionId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            _bookTransactionsServiceMock.Verify();
        }

        [Fact]
        public async Task AcceptReturn_InvalidTransactionId_ReturnsBadRequest()
        {
            // Arrange
            int transactionId = 1;

            _bookTransactionsServiceMock.Setup(s => s.AcceptReturnAsync(transactionId))
                .ThrowsAsync(new Exception("Book transaction not found"));

            // Act
            var result = await _bookTransactionsController.AcceptReturn(transactionId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Book transaction not found", badRequestObjectResult.Value);

            _bookTransactionsServiceMock.Verify();
        }
    }
}
