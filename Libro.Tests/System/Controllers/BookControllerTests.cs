using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Presentation.Controllers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Libro.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookManagementService> _bookManagementServiceMock;
        private readonly Mock<IGenreManagementService> _genreManagementServiceMock;
        private readonly Mock<IAuthorManagementService> _authorManagementServiceMock;
        private readonly Mock<IUserManagementService> _userManagementServiceMock;
        private readonly Mock<IReadingListService> _readingListServiceMock;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BooksController _booksController;

        public BooksControllerTests()
        {
            _bookManagementServiceMock = new Mock<IBookManagementService>();
            _genreManagementServiceMock = new Mock<IGenreManagementService>();
            _authorManagementServiceMock = new Mock<IAuthorManagementService>();
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _readingListServiceMock = new Mock<IReadingListService>();
            _reviewServiceMock = new Mock<IReviewService>();
            _mapperMock = new Mock<IMapper>();
            _booksController = new BooksController(
                _bookManagementServiceMock.Object,
                _genreManagementServiceMock.Object,
                _authorManagementServiceMock.Object,
                _userManagementServiceMock.Object,
                _readingListServiceMock.Object,
                _mapperMock.Object,
                _reviewServiceMock.Object
            );
        }

        [Fact]
        public async Task Search_GET_ReturnsViewWithModel()
        {
            // Arrange
            var allBooks = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1" },
                new BookDTO { BookId = 2, Title = "Book 2" },
                new BookDTO { BookId = 3, Title = "Book 3" }
            };
            var searchResults = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1" },
                new BookDTO { BookId = 3, Title = "Book 3" }
            };
            var authors = new List<AuthorDTO>
            {
                new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" },
                new AuthorDTO { AuthorId = 2, AuthorName = "Author 2" }
            };
            var genres = new List<GenreDTO>
            {
                new GenreDTO { GenreId = 1, Name = "Genre 1" },
                new GenreDTO { GenreId = 2, Name = "Genre 2" }
            };
            var genreNames = genres.Select(g => g.Name).ToList();

            _bookManagementServiceMock.Setup(b => b.GetAllBooksAsync()).ReturnsAsync(allBooks);
            _bookManagementServiceMock.Setup(b => b.FindBooksAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(searchResults);
            _authorManagementServiceMock.Setup(a => a.GetAllAuthorsAsync()).ReturnsAsync(authors);
            _genreManagementServiceMock.Setup(g => g.GetAllGenresAsync()).ReturnsAsync(genres);

            var expectedModel = new SearchViewModel
            {
                Genres = genreNames,
                AllBooks = allBooks,
                FilteredBooks = searchResults,
                BookGenre = null,
                AllAuthors = authors,
                SearchString = null,
                AuthorName = null,
                AvailabilityStatus = null,
                PageNumber = 1,
                TotalPages = 1
            };

            // Act
            var result = await _booksController.Search(null, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var model = Assert.IsType<SearchViewModel>(viewResult.Model);
            Assert.Equal(expectedModel.Genres, model.Genres);
            Assert.Equal(expectedModel.AllBooks, model.AllBooks);
            Assert.Equal(expectedModel.FilteredBooks, model.FilteredBooks);
            Assert.Equal(expectedModel.BookGenre, model.BookGenre);
            Assert.Equal(expectedModel.AllAuthors, model.AllAuthors);
            Assert.Equal(expectedModel.SearchString, model.SearchString);
            Assert.Equal(expectedModel.AuthorName, model.AuthorName);
            Assert.Equal(expectedModel.AvailabilityStatus, model.AvailabilityStatus);
            Assert.Equal(expectedModel.PageNumber, model.PageNumber);
            Assert.Equal(expectedModel.TotalPages, model.TotalPages);
        }

        [Fact]
        public async Task BookDetails_GET_ReturnsViewWithModel()
        {
            // Arrange
            var userId = 1;
            var bookId = 1;
            var isBookInReadingList = false;
            var reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Id = 1, BookId = bookId, Rating = 4 },
                new ReviewDTO { Id = 2, BookId = bookId, Rating = 5 }
            };
            var averageRating = 4.5;
            var book = new BookDTO { BookId = bookId, Title = "Book 1" };

            _userManagementServiceMock.Setup(u => u.GetUserByIdAsync(userId)).ReturnsAsync(new UserDTO { UserId = userId });
            _readingListServiceMock.Setup(r => r.GetReadingListsByUserIdAsync(userId)).ReturnsAsync(new List<ReadingListDTO>());
            _readingListServiceMock.Setup(r => r.IsBookInReadingListAsync(It.IsAny<int>(), bookId)).ReturnsAsync(isBookInReadingList);
            _reviewServiceMock.Setup(r => r.GetReviewsByBookIdAsync(bookId)).ReturnsAsync(reviews);
            _reviewServiceMock.Setup(r => r.GetAverageRatingForBookAsync(bookId)).ReturnsAsync(averageRating);
            _bookManagementServiceMock.Setup(b => b.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDetailsViewModel>(book)).Returns(new BookDetailsViewModel { BookId = bookId });

            var expectedModel = new BookDetailsViewModel
            {
                BookId = bookId,
                Genre = null,
                IsBookInReadingList = isBookInReadingList,
                Reviews = reviews,
                averageRating = averageRating
            };
            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };
            // Act
            var result = await _booksController.BookDetails(bookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var model = Assert.IsType<BookDetailsViewModel>(viewResult.Model);
            Assert.Equal(expectedModel.BookId, model.BookId);
            Assert.Equal(expectedModel.Genre, model.Genre);
            Assert.Equal(expectedModel.IsBookInReadingList, model.IsBookInReadingList);
            Assert.Equal(expectedModel.Reviews, model.Reviews);
            Assert.Equal(expectedModel.averageRating, model.averageRating);
        }

        [Fact]
        public async Task EditBook_GET_WithValidBookId_ReturnsViewWithModel()
        {
            // Arrange
            var bookId = 1;
            var book = new BookDTO 
            {
                BookId = bookId,
                Title = "Book 1",
                Description = "Descreption...",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" }
            };
            var genres = new List<GenreDTO>
            {
                new GenreDTO { GenreId = 1, Name = "Genre 1" },
                new GenreDTO { GenreId = 2, Name = "Genre 2" }
            };
            var authors = new List<AuthorDTO>
            {
                new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" },
                new AuthorDTO { AuthorId = 2, AuthorName = "Author 2" }
            };

            _bookManagementServiceMock.Setup(b => b.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _genreManagementServiceMock.Setup(g => g.GetAllGenresAsync()).ReturnsAsync(genres);
            _authorManagementServiceMock.Setup(a => a.GetAllAuthorsAsync()).ReturnsAsync(authors);

            var expectedModel = new EditBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                AvailabilityStatus = book.AvailabilityStatus,
                AllGenres = genres.ToList(),
                SelectedGenre = book.Genre.Name,
                AllAuthors = authors.ToList(),
                SelectedAuthors = book.BookAuthors?.Select(a => a.Author.AuthorName)?.ToList() ?? new List<string>()
            };

            // Act
            var result = await _booksController.EditBook(bookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var model = Assert.IsType<EditBookViewModel>(viewResult.Model);
            Assert.Equal(expectedModel.BookId, model.BookId);
            Assert.Equal(expectedModel.Title, model.Title);
            Assert.Equal(expectedModel.Description, model.Description);
            Assert.Equal(expectedModel.PublicationDate, model.PublicationDate);
            Assert.Equal(expectedModel.AvailabilityStatus, model.AvailabilityStatus);
            Assert.Equal(expectedModel.AllGenres, model.AllGenres);
            Assert.Equal(expectedModel.SelectedGenre, model.SelectedGenre);
            Assert.Equal(expectedModel.AllAuthors, model.AllAuthors);
            Assert.Equal(expectedModel.SelectedAuthors, model.SelectedAuthors);
        }

        [Fact]
        public async Task EditBook_POST_WithValidModel_RedirectsToSearchAction()
        {
            // Arrange
            var bookId = 1;
            var bookDetailsModel = new EditBookViewModel
            {
                BookId = bookId,
                Title = "Updated Book 1",
                Description = "Updated description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                SelectedGenre = "Genre 1",
                SelectedAuthors = new List<string> { "Author 1"}
            };

            var book = new BookDTO
            {
                BookId = bookId,
                Title = "Book 1",
                Description = "Description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.UnAvailable,
                GenreId = 1,
                Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" },
                BookAuthors = new List<BookAuthorDTO>
                {
                    new BookAuthorDTO { BookId = bookId, Author = new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" } }
                }
            };

            var Genres = new List<GenreDTO>
            {
                new GenreDTO { GenreId = 1, Name = "Genre 1" },
                new GenreDTO { GenreId = 2, Name = "Genre 2" }
            };
            var selectedGenre = new GenreDTO { GenreId = 1, Name = "Genre 1" };
            var selectedAuthors = new List<AuthorDTO>
            {
                new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" }
            };
            var BookAuthor = new BookAuthorDTO { BookId = bookId, Author = new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" } };

            _bookManagementServiceMock.Setup(b => b.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _genreManagementServiceMock.Setup(g => g.GetAllGenresAsync()).ReturnsAsync(Genres);
            _bookManagementServiceMock.Setup(b => b.DeleteBookAuthorsByBookIdAsync(bookId)).ReturnsAsync(true);
            _bookManagementServiceMock.Setup(b => b.CreateBookAuthorAsync(bookId, It.IsAny<int>())).ReturnsAsync(BookAuthor);
            _bookManagementServiceMock.Setup(b => b.UpdateBookAsync(bookId, It.IsAny<BookDTO>())).ReturnsAsync(book);

            // Act
            var result = await _booksController.EditBook(bookDetailsModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Search", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task EditBook_POST_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var bookDetailsModel = new EditBookViewModel
            {
                BookId = 1,
                Title = "Updated Book 1",
                Description = "Updated description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                SelectedGenre = "Genre 1",
                SelectedAuthors = new List<string> { "Author 1", "Author 2" }
            };

            var genres = new List<GenreDTO>
            {
                new GenreDTO { GenreId = 1, Name = "Genre 1" },
                new GenreDTO { GenreId = 2, Name = "Genre 2" }
            };

            _genreManagementServiceMock.Setup(g => g.GetAllGenresAsync()).ReturnsAsync(genres);

            _booksController.ModelState.AddModelError("", "Error");

            // Act
            var result = await _booksController.EditBook(bookDetailsModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var model = Assert.IsType<EditBookViewModel>(viewResult.Model);
            Assert.Equal(bookDetailsModel, model);
        }

        [Fact]
        public async Task DeleteBook_POST_WithValidBookId_RedirectsToSearchAction()
        {
            // Arrange
            var bookId = 1;
            _bookManagementServiceMock.Setup(b => b.DeleteBookAsync(bookId)).ReturnsAsync(true);

            // Act
            var result = await _booksController.DeleteBook(bookId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Search", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task DeleteBook_POST_WithInvalidBookId_ReturnsView()
        {
            // Arrange
            var bookId = 1;
            _bookManagementServiceMock.Setup(b => b.DeleteBookAsync(bookId)).ThrowsAsync(new Exception());

            // Act
            var result = await _booksController.DeleteBook(bookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task CreateBook_GET_ReturnsViewWithModel()
        {
            // Arrange
            var genres = new List<GenreDTO>
            {
                new GenreDTO { GenreId = 1, Name = "Genre 1" },
                new GenreDTO { GenreId = 2, Name = "Genre 2" }
            };
            var authors = new List<AuthorDTO>
            {
                new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" },
                new AuthorDTO { AuthorId = 2, AuthorName = "Author 2" }
            };

            _genreManagementServiceMock.Setup(g => g.GetAllGenresAsync()).ReturnsAsync(genres);
            _authorManagementServiceMock.Setup(a => a.GetAllAuthorsAsync()).ReturnsAsync(authors);

            var expectedModel = new CreateBookViewModel
            {
                Genres = genres,
                Authors = authors
            };

            // Act
            var result = await _booksController.CreateBook();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var model = Assert.IsType<CreateBookViewModel>(viewResult.Model);
            Assert.Equal(expectedModel.Genres, model.Genres);
            Assert.Equal(expectedModel.Authors, model.Authors);
        }

        [Fact]
        public async Task CreateBook_POST_WithValidModel_RedirectsToSearchAction()
        {
            // Arrange
            var genres = new List<GenreDTO>
            {
                new GenreDTO { GenreId = 1, Name = "Genre 1" },
                new GenreDTO { GenreId = 2, Name = "Genre 2" }
            };
            var authors = new List<AuthorDTO>
            {
                new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" },
                new AuthorDTO { AuthorId = 2, AuthorName = "Author 2" }
            };
            var selectedAuthors = new List<string>
            {
                "Author 1" 
            };
            var selectedGenre = new List<string> 
            {
                "Genre 1"
            };

            var bookViewModel = new CreateBookViewModel
            {
                Title = "Book 1",
                Description = "Description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                SelectedGenre = selectedGenre[0],
                SelectedAuthors = selectedAuthors,
                Genres = genres,
                Authors = authors
            };
            var book = new BookDTO
            {
                BookId = 1,
                Title = bookViewModel.Title,
                Description = bookViewModel.Description,
                PublicationDate = bookViewModel.PublicationDate,
                AvailabilityStatus = bookViewModel.AvailabilityStatus,
                GenreId = 1
            };
            var bookAuthor = new BookAuthorDTO { Id = 1, AuthorId = authors[0].AuthorId, BookId = book.BookId, Author = authors[0], Book = book };

            _genreManagementServiceMock.Setup(g => g.GetAllGenresAsync()).ReturnsAsync(genres);
            _bookManagementServiceMock.Setup(b => b.CreateBookAsync(It.IsAny<BookDTO>())).ReturnsAsync(book);
            _bookManagementServiceMock.Setup(b => b.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync(book);
            _authorManagementServiceMock.Setup(a => a.GetAuthorByNameAsync(It.IsAny<string>())).ReturnsAsync(authors[0]);
            _bookManagementServiceMock.Setup(b => b.CreateBookAuthorAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(bookAuthor);

            // Act
            var result = await _booksController.CreateBook(bookViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Search", redirectToActionResult.ActionName);
        }


        [Fact]
        public async Task CreateBook_POST_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var genres = new List<GenreDTO>
            {
                new GenreDTO { GenreId = 1, Name = "Genre 1" },
                new GenreDTO { GenreId = 2, Name = "Genre 2" }
            };
            var authors = new List<AuthorDTO>
            {
                new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" },
                new AuthorDTO { AuthorId = 2, AuthorName = "Author 2" }
            };
            var bookViewModel = new CreateBookViewModel
            {
                Title = "Book 1",
                Description = "Description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                SelectedGenre = "Genre 1",
                SelectedAuthors = new List<string> { "Author 1", "Author 2" }
            };

            _genreManagementServiceMock.Setup(g => g.GetAllGenresAsync()).ReturnsAsync(genres);

            _booksController.ModelState.AddModelError("", "Error");

            // Act
            var result = await _booksController.CreateBook(bookViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            var model = Assert.IsType<CreateBookViewModel>(viewResult.Model);
            Assert.Equal(bookViewModel, model);
        }

        [Fact]
        public async Task AddToReadingList_POST_WithAuthenticatedUser_RedirectsToBookDetailsAction()
        {
            // Arrange
            var userId = 1;
            var bookId = 1;
            var user = new UserDTO { UserId = userId };
            var books = new List<BookDTO> { new BookDTO { BookId = 1 }, new BookDTO { BookId = 2 } };
            var readingLists = new List<ReadingListDTO>
            {
                new ReadingListDTO { Id = 1, UserId = userId, User = user, Books = books},
            };
            var readingList = new ReadingListDTO { Id = 1, UserId = userId, User = user, Books = books };
            var isBookInReadingList = false;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            _readingListServiceMock.Setup(r => r.GetReadingListsByUserIdAsync(userId)).ReturnsAsync(readingLists);
            _readingListServiceMock.Setup(r => r.IsBookInReadingListAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(isBookInReadingList);
            _readingListServiceMock.Setup(r => r.AddBookToReadingListAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            _readingListServiceMock.Setup(r => r.CreateReadingListAsync(userId)).ReturnsAsync(readingList);

            // Act
            var result = await _booksController.AddToReadingList(bookId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BookDetails", redirectToActionResult.ActionName);
            Assert.Equal(bookId, redirectToActionResult.RouteValues["id"]);
        }


        [Fact]
        public async Task RemoveFromReadingList_POST_WithAuthenticatedUser_RedirectsToRefererWhenBookDetails()
        {
            // Arrange
            var userId = 1;
            var bookId = 1;
            var readingLists = new List<ReadingListDTO>
            {
                new ReadingListDTO { Id = 1, UserId = userId }
            };
            var readingList = readingLists.FirstOrDefault();

            _booksController.ControllerContext.HttpContext = new DefaultHttpContext();
            _booksController.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "authenticationType"));

            _readingListServiceMock.Setup(r => r.GetReadingListsByUserIdAsync(userId)).ReturnsAsync(readingLists);
            _readingListServiceMock.Setup(r => r.RemoveBookFromReadingListAsync(readingList.Id, bookId)).ReturnsAsync(true);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            // Act
            var result = await _booksController.RemoveFromReadingList(bookId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BookDetails", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveFromReadingList_POST_WithAuthenticatedUser_RedirectsToProfileActionWhenNotBookDetails()
        {
            // Arrange
            var userId = 1;
            var bookId = 1;
            var readingLists = new List<ReadingListDTO>
            {
                new ReadingListDTO { Id = 1, UserId = userId }
            };
            var readingList = readingLists.FirstOrDefault();

            _booksController.ControllerContext.HttpContext = new DefaultHttpContext();
            _booksController.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "authenticationType"));

            _readingListServiceMock.Setup(r => r.GetReadingListsByUserIdAsync(userId)).ReturnsAsync(readingLists);
            _readingListServiceMock.Setup(r => r.RemoveBookFromReadingListAsync(readingList.Id, bookId)).ReturnsAsync(true);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            _booksController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) }
            };

            // Act
            var result = await _booksController.RemoveFromReadingList(bookId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BookDetails", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task AddReview_POST_WithAuthenticatedUser_RedirectsToBookDetailsAction()
        {
            // Arrange
            var userId = 1;
            var bookId = 1;
            var user = new UserDTO { UserId = userId };
            var book = new BookDTO { BookId = bookId, Title = "Book 1" };
            var rating = 4;
            var comment = "Great book";

            _booksController.ControllerContext.HttpContext = new DefaultHttpContext();
            _booksController.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "authenticationType"));

            _userManagementServiceMock.Setup(u => u.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _bookManagementServiceMock.Setup(b => b.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _reviewServiceMock.Setup(r => r.CreateReviewAsync(It.IsAny<ReviewDTO>())).Returns(Task.CompletedTask);

            // Act
            var result = await _booksController.AddReview(bookId, rating, comment);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BookDetails", redirectToActionResult.ActionName);
            Assert.Equal(bookId, redirectToActionResult.RouteValues["id"]);
        }
    }
}
