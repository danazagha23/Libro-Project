using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Libro.Tests.Services
{
    public class BookTransactionsServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IBookTransactionsRepository> _transactionsRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BookTransactionsService>> _loggerMock;
        private readonly BookTransactionsService _bookTransactionsService;

        public BookTransactionsServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _transactionsRepositoryMock = new Mock<IBookTransactionsRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BookTransactionsService>>();

            _bookTransactionsService = new BookTransactionsService(
                _bookRepositoryMock.Object,
                _userRepositoryMock.Object,
                _transactionsRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldCreateTransactionAndReturnTrue()
        {
            // Arrange
            var transactionDTO = new BookTransactionDTO
            {
                TransactionId = 1,
                PatronId = 1,
                BookId = 1,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Borrowed,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false
            };

            var transaction = new BookTransaction
            {
                TransactionId = 1,
                PatronId = 1,
                BookId = 1,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Borrowed,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false
            };

            _mapperMock.Setup(mapper => mapper.Map<BookTransaction>(transactionDTO)).Returns(transaction);
            _transactionsRepositoryMock.Setup(repo => repo.CreateTransactionAsync(transaction)).ReturnsAsync(transaction);

            // Act
            var result = await _bookTransactionsService.CreateTransactionAsync(transactionDTO);

            // Assert
            _mapperMock.Verify(mapper => mapper.Map<BookTransaction>(transactionDTO), Times.Once);
            _transactionsRepositoryMock.Verify(repo => repo.CreateTransactionAsync(transaction), Times.Once);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteTransactionAsync_WithValidTransactionId_ShouldDeleteTransactionAndReturnTrue()
        {
            // Arrange
            var transactionId = 1;

            _transactionsRepositoryMock.Setup(repo => repo.DeleteTransactionAsync(transactionId)).ReturnsAsync(true);

            // Act
            var result = await _bookTransactionsService.DeleteTransactionAsync(transactionId);

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.DeleteTransactionAsync(transactionId), Times.Once);

            Assert.True(result);
        }

        [Fact]
        public async Task GetAllBookTransactionsAsync_ShouldReturnAllBookTransactions()
        {
            // Arrange
            var transactions = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = 1, BookId = 1 },
                new BookTransaction { TransactionId = 2, PatronId = 2, BookId = 2 }
            };

            _transactionsRepositoryMock.Setup(repo => repo.GetAllBookTransactionsAsync()).ReturnsAsync(transactions);

            var expectedTransactionDTOs = new List<BookTransactionDTO>
            {
                new BookTransactionDTO { TransactionId = 1, PatronId = 1, BookId = 1 },
                new BookTransactionDTO { TransactionId = 2, PatronId = 2, BookId = 2 }
            };

            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<BookTransactionDTO>>(transactions)).Returns(expectedTransactionDTOs);

            // Act
            var result = await _bookTransactionsService.GetAllBookTransactionsAsync();

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.GetAllBookTransactionsAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<IEnumerable<BookTransactionDTO>>(transactions), Times.Once);

            Assert.Equal(expectedTransactionDTOs, result);
        }

        [Fact]
        public async Task ReserveBookAsync_WithAvailableBook_ShouldCreateTransactionAndSetBookAvailabilityStatusToReserved()
        {
            // Arrange
            var bookId = 1;
            var patronId = 1;

            var book = new Book { BookId = bookId, AvailabilityStatus = AvailabilityStatus.Available };
            var patron = new User { UserId = patronId };

            _bookRepositoryMock.Setup(repo => repo.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(patronId)).ReturnsAsync(patron);

            var transaction = new BookTransaction
            {
                TransactionId = 1,
                PatronId = patronId,
                BookId = bookId,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Reserved,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false
            };
            var updatedBook = new Book { BookId = bookId, AvailabilityStatus = AvailabilityStatus.Reserved };

            _transactionsRepositoryMock.Setup(repo => repo.CreateTransactionAsync(transaction)).ReturnsAsync(transaction);
            _bookRepositoryMock.Setup(repo => repo.UpdateBookAsync(book.BookId, book)).ReturnsAsync(updatedBook);

            // Act
            await _bookTransactionsService.ReserveBookAsync(bookId, patronId);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(bookId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(patronId), Times.Once);
            _transactionsRepositoryMock.Verify(repo => repo.CreateTransactionAsync(It.IsAny<BookTransaction>()), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(bookId, book), Times.Once);

            Assert.Equal(AvailabilityStatus.Reserved, updatedBook.AvailabilityStatus);
        }

        [Fact]
        public async Task ReserveBookAsync_WithUnavailableBook_ShouldThrowException()
        {
            // Arrange
            var bookId = 1;
            var patronId = 1;

            var book = new Book { BookId = bookId, AvailabilityStatus = AvailabilityStatus.Borrowed };
            var patron = new User { UserId = patronId };

            _bookRepositoryMock.Setup(repo => repo.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(patronId)).ReturnsAsync(patron);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookTransactionsService.ReserveBookAsync(bookId, patronId));

            // Assert
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(bookId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(patronId), Times.Once);
            _transactionsRepositoryMock.Verify(repo => repo.CreateTransactionAsync(It.IsAny<BookTransaction>()), Times.Never);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(It.IsAny<int>(), It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CheckOutBookAsync_WithValidTransactionId_ShouldUpdateTransactionAndSetBookAvailabilityStatusToBorrowed()
        {
            // Arrange
            var transactionId = 1;

            var transaction = new BookTransaction
            {
                TransactionId = transactionId,
                TransactionType = TransactionType.Reserved,
                IsReturned = false,
                BookId = 1
            };

            var book = new Book { BookId = 1, AvailabilityStatus = AvailabilityStatus.Reserved };

            _transactionsRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);
            _bookRepositoryMock.Setup(repo => repo.GetBookByIdAsync(transaction.BookId)).ReturnsAsync(book);
            _transactionsRepositoryMock.Setup(repo => repo.UpdateTransactionAsync(transactionId, transaction)).ReturnsAsync(transaction);
            _bookRepositoryMock.Setup(repo => repo.UpdateBookAsync(book.BookId, book)).ReturnsAsync(book);

            // Act
            await _bookTransactionsService.CheckOutBookAsync(transactionId);

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(transactionId), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(transaction.BookId), Times.Once);
            _transactionsRepositoryMock.Verify(repo => repo.UpdateTransactionAsync(transactionId, transaction), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(book.BookId, book), Times.Once);

            Assert.Equal(TransactionType.Borrowed, transaction.TransactionType);
            Assert.Equal(AvailabilityStatus.Borrowed, book.AvailabilityStatus);
        }

        [Fact]
        public async Task CheckOutBookAsync_WithInvalidTransactionId_ShouldThrowException()
        {
            // Arrange
            var transactionId = 1;

            _transactionsRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(transactionId)).ReturnsAsync((BookTransaction)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookTransactionsService.CheckOutBookAsync(transactionId));

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(transactionId), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(It.IsAny<int>()), Times.Never);
            _transactionsRepositoryMock.Verify(repo => repo.UpdateTransactionAsync(It.IsAny<int>(), It.IsAny<BookTransaction>()), Times.Never);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(It.IsAny<int>(), It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task CheckOutBookAsync_WithInvalidTransactionType_ShouldThrowException()
        {
            // Arrange
            var transactionId = 1;

            var transaction = new BookTransaction
            {
                TransactionId = transactionId,
                TransactionType = TransactionType.Borrowed,
                IsReturned = false,
                BookId = 1
            };

            _transactionsRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookTransactionsService.CheckOutBookAsync(transactionId));

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(transactionId), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(It.IsAny<int>()), Times.Never);
            _transactionsRepositoryMock.Verify(repo => repo.UpdateTransactionAsync(It.IsAny<int>(), It.IsAny<BookTransaction>()), Times.Never);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(It.IsAny<int>(), It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task AcceptReturnAsync_WithValidTransactionId_ShouldUpdateTransactionAndSetBookAvailabilityStatusToAvailable()
        {
            // Arrange
            var transactionId = 1;

            var transaction = new BookTransaction
            {
                TransactionId = transactionId,
                TransactionType = TransactionType.Borrowed,
                IsReturned = false,
                BookId = 1
            };

            var book = new Book { BookId = 1, AvailabilityStatus = AvailabilityStatus.Borrowed };

            _transactionsRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);
            _bookRepositoryMock.Setup(repo => repo.GetBookByIdAsync(transaction.BookId)).ReturnsAsync(book);
            _transactionsRepositoryMock.Setup(repo => repo.UpdateTransactionAsync(transactionId, transaction)).ReturnsAsync(transaction);
            _bookRepositoryMock.Setup(repo => repo.UpdateBookAsync(book.BookId, book)).ReturnsAsync(book);

            // Act
            await _bookTransactionsService.AcceptReturnAsync(transactionId);

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(transactionId), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(transaction.BookId), Times.Once);
            _transactionsRepositoryMock.Verify(repo => repo.UpdateTransactionAsync(transactionId, transaction), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(book.BookId, book), Times.Once);

            Assert.Equal(TransactionType.Returned, transaction.TransactionType);
            Assert.True(transaction.IsReturned);
            Assert.Equal(AvailabilityStatus.Available, book.AvailabilityStatus);
        }

        [Fact]
        public async Task AcceptReturnAsync_WithInvalidTransactionId_ShouldThrowException()
        {
            // Arrange
            var transactionId = 1;

            _transactionsRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(transactionId)).ReturnsAsync((BookTransaction)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookTransactionsService.AcceptReturnAsync(transactionId));

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(transactionId), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(It.IsAny<int>()), Times.Never);
            _transactionsRepositoryMock.Verify(repo => repo.UpdateTransactionAsync(It.IsAny<int>(), It.IsAny<BookTransaction>()), Times.Never);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(It.IsAny<int>(), It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task AcceptReturnAsync_WithInvalidTransactionType_ShouldThrowException()
        {
            // Arrange
            var transactionId = 1;

            var transaction = new BookTransaction
            {
                TransactionId = transactionId,
                TransactionType = TransactionType.Reserved,
                IsReturned = false,
                BookId = 1
            };

            _transactionsRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookTransactionsService.AcceptReturnAsync(transactionId));

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(transactionId), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.GetBookByIdAsync(It.IsAny<int>()), Times.Never);
            _transactionsRepositoryMock.Verify(repo => repo.UpdateTransactionAsync(It.IsAny<int>(), It.IsAny<BookTransaction>()), Times.Never);
            _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(It.IsAny<int>(), It.IsAny<Book>()), Times.Never);
        }

        [Fact]
        public async Task FindTransactionsAsync_ShouldReturnSearchResults()
        {
            // Arrange
            var selectedType = "Borrowed";
            var selectedPatron = "John Doe";
            var selectedBook = "Book Title";

            var searchResults = new List<BookTransaction>
            {
                new BookTransaction { TransactionId = 1, PatronId = 1, BookId = 1 },
                new BookTransaction { TransactionId = 2, PatronId = 2, BookId = 2 }
            };

            _transactionsRepositoryMock.Setup(repo => repo.FindTransactionsAsync(selectedType, selectedPatron, selectedBook)).ReturnsAsync(searchResults);

            var expectedTransactionDTOs = new List<BookTransactionDTO>
            {
                new BookTransactionDTO { TransactionId = 1, PatronId = 1, BookId = 1 },
                new BookTransactionDTO { TransactionId = 2, PatronId = 2, BookId = 2 }
            };

            _mapperMock.Setup(mapper => mapper.Map<ICollection<BookTransactionDTO>>(searchResults)).Returns(expectedTransactionDTOs);

            // Act
            var result = await _bookTransactionsService.FindTransactionsAsync(selectedType, selectedPatron, selectedBook);

            // Assert
            _transactionsRepositoryMock.Verify(repo => repo.FindTransactionsAsync(selectedType, selectedPatron, selectedBook), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<ICollection<BookTransactionDTO>>(searchResults), Times.Once);

            Assert.Equal(expectedTransactionDTOs, result);
        }
    }
}
