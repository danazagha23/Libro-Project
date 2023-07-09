using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
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
    public class BooksControllerTests
    {
        private readonly Mock<IBookManagementService> _bookServiceMock;
        private readonly Mock<IGenreManagementService> _genreServiceMock;
        private readonly Mock<IAuthorManagementService> _authorServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _bookServiceMock = new Mock<IBookManagementService>();
            _genreServiceMock = new Mock<IGenreManagementService>();
            _authorServiceMock = new Mock<IAuthorManagementService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new BooksController(_bookServiceMock.Object, _genreServiceMock.Object, _authorServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Search_ReturnsViewResultWithSearchViewModel()
        {
            // Arrange
            var bookGenre = "Genre 1";
            var searchString = "Book 1";
            var authorName = "Author 1";
            var availabilityStatus = "Available";
            var page = 1;
            var pageSize = 5;

            var allBooks = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1", Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" } },
                new BookDTO { BookId = 2, Title = "Book 2", Genre = new GenreDTO { GenreId = 2, Name = "Genre 2" } },
                new BookDTO { BookId = 3, Title = "Book 3", Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" } }
            };
            var searchResults = new List<BookDTO>
            {
                new BookDTO { BookId = 1, Title = "Book 1", Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" } }
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

            _bookServiceMock.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(allBooks);
            _bookServiceMock.Setup(x => x.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus))
                .ReturnsAsync(searchResults);
            _authorServiceMock.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(authors);
            _genreServiceMock.Setup(x => x.GetAllGenresAsync()).ReturnsAsync(genres);

            var pagedAllBooks = allBooks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pagedFilteredBooks = searchResults.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var expectedViewModel = new SearchViewModel
            {
                Genres = genres.Select(g => g.Name).ToList(),
                AllBooks = pagedAllBooks,
                FilteredBooks = pagedFilteredBooks,
                BookGenre = bookGenre,
                AllAuthors = authors,
                SearchString = searchString,
                AuthorName = authorName,
                AvailabilityStatus = availabilityStatus,
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)searchResults.Count() / pageSize)
            };

            // Act
            var result = await _controller.Search(bookGenre, searchString, authorName, availabilityStatus, page, pageSize) as ViewResult;
            var model = result?.Model as SearchViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedViewModel.Genres, model?.Genres);
            Assert.Equal(expectedViewModel.AllBooks, model?.AllBooks);
            Assert.Equal(expectedViewModel.FilteredBooks, model?.FilteredBooks);
            Assert.Equal(expectedViewModel.BookGenre, model?.BookGenre);
            Assert.Equal(expectedViewModel.AllAuthors, model?.AllAuthors);
            Assert.Equal(expectedViewModel.SearchString, model?.SearchString);
            Assert.Equal(expectedViewModel.AuthorName, model?.AuthorName);
            Assert.Equal(expectedViewModel.AvailabilityStatus, model?.AvailabilityStatus);
            Assert.Equal(expectedViewModel.PageNumber, model?.PageNumber);
            Assert.Equal(expectedViewModel.TotalPages, model?.TotalPages);
        }


        [Fact]
        public async Task BookDetails_ExistingBookId_ReturnsViewResultWithBookDetailsViewModel()
        {
            // Arrange
            var bookId = 1;
            var book = new BookDTO { BookId = bookId, Title = "Book 1" };
            var bookDetailsViewModel = new BookDetailsViewModel { BookId = bookId, Title = "Book 1" };

            _bookServiceMock.Setup(x => x.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(x => x.Map<BookDetailsViewModel>(book)).Returns(bookDetailsViewModel);

            // Act
            var result = await _controller.BookDetails(bookId) as ViewResult;
            var model = result?.Model as BookDetailsViewModel;

            // AssertAssert
            Assert.NotNull(result);
            Assert.Equal(bookDetailsViewModel.BookId, model?.BookId);
            Assert.Equal(bookDetailsViewModel.Title, model?.Title);
        }

        [Fact]
        public async Task BookDetails_NonExistingBookId_ReturnsNotFound()
        {
            // Arrange
            var bookId = 1;
            BookDTO book = null;

            _bookServiceMock.Setup(x => x.GetBookByIdAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _controller.BookDetails(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task EditBook_ExistingBookId_ReturnsViewResultWithEditBookViewModel()
        {
            // Arrange
            var bookId = 1;
            var book = new BookDTO { BookId = bookId, Title = "Book 1", Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" } };
            var genres = new List<GenreDTO> { new GenreDTO { GenreId = 1, Name = "Genre 1" }, new GenreDTO { GenreId = 2, Name = "Genre 2" } };
            var authors = new List<AuthorDTO> { new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" } };

            _bookServiceMock.Setup(x => x.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _genreServiceMock.Setup(x => x.GetAllGenresAsync()).ReturnsAsync(genres);
            _authorServiceMock.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(authors);

            var expectedViewModel = new EditBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                AvailabilityStatus = book.AvailabilityStatus,
                SelectedGenre = book.Genre.Name,
                AllAuthors = authors.ToList(),
                SelectedAuthors = book.BookAuthors?.Select(a => a.Author.AuthorName)?.ToList() ?? new List<string>()
            };

            // Act
            var result = await _controller.EditBook(bookId) as ViewResult;
            var model = result?.Model as EditBookViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedViewModel.BookId, model?.BookId);
            Assert.Equal(expectedViewModel.Title, model?.Title);
            Assert.Equal(expectedViewModel.Description, model?.Description);
            Assert.Equal(expectedViewModel.PublicationDate, model?.PublicationDate);
            Assert.Equal(expectedViewModel.AvailabilityStatus, model?.AvailabilityStatus);
            Assert.Equal(expectedViewModel.SelectedGenre, model?.SelectedGenre);
            Assert.Equal(expectedViewModel.AllAuthors.Count, model?.AllAuthors.Count);
            Assert.Equal(expectedViewModel.SelectedAuthors, model?.SelectedAuthors);
        }

        [Fact]
        public async Task EditBook_ValidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            var bookDetailsModel = new EditBookViewModel
            {
                BookId = 1,
                Title = "Updated Book Title",
                Description = "Updated Book Description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                SelectedGenre = "Genre 1",
                SelectedAuthors = new List<string> { "Author 1" }
            };

            var book = new BookDTO
            {
                BookId = bookDetailsModel.BookId,
                Title = "Original Book Title",
                Description = "Original Book Description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                GenreId = 1,
                Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" },
                BookAuthors = new List<BookAuthorDTO>
        {
            new BookAuthorDTO
            {
                BookId = 1,
                Author = new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" }
            }
        }
            };

            _bookServiceMock.Setup(x => x.GetBookByIdAsync(bookDetailsModel.BookId)).ReturnsAsync(book);
            _genreServiceMock.Setup(x => x.GetAllGenresAsync()).ReturnsAsync(new List<GenreDTO>());
            _authorServiceMock.Setup(x => x.GetAuthorByNameAsync("Author 1")).ReturnsAsync(new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" });
            _bookServiceMock.Setup(x => x.DeleteBookAuthorsByBookIdAsync(book.BookId)).ReturnsAsync(It.IsAny<bool>);
            _bookServiceMock.Setup(x => x.CreateBookAuthorAsync(book.BookId, 1)).ReturnsAsync(new BookAuthorDTO());
            _bookServiceMock.Setup(x => x.UpdateBookAsync(book.BookId, It.IsAny<BookDTO>())).ReturnsAsync(new BookDTO());

            // Act
            var result = await _controller.EditBook(bookDetailsModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Search", result.ActionName);
            _bookServiceMock.Verify(x => x.GetBookByIdAsync(bookDetailsModel.BookId), Times.Once);
            _genreServiceMock.Verify(x => x.GetAllGenresAsync(), Times.Once);
            _authorServiceMock.Verify(x => x.GetAuthorByNameAsync("Author 1"), Times.Once);
            _bookServiceMock.Verify(x => x.DeleteBookAuthorsByBookIdAsync(book.BookId), Times.Once);
            _bookServiceMock.Verify(x => x.CreateBookAuthorAsync(book.BookId, 1), Times.Once);
            _bookServiceMock.Verify(x => x.UpdateBookAsync(book.BookId, It.IsAny<BookDTO>()), Times.Once);
        }


        [Fact]
        public async Task EditBook_InvalidModel_ReturnsViewResultWithEditBookViewModel()
        {
            // Arrange
            var bookId = 1;
            var book = new BookDTO { BookId = bookId, Title = "Book 1", Genre = new GenreDTO { GenreId = 1, Name = "Genre 1" } };
            var genres = new List<GenreDTO> { new GenreDTO { GenreId = 1, Name = "Genre 1" }, new GenreDTO { GenreId = 2, Name = "Genre 2" } };
            var authors = new List<AuthorDTO> { new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" } };

            _bookServiceMock.Setup(x => x.GetBookByIdAsync(bookId)).ReturnsAsync(book);
            _genreServiceMock.Setup(x => x.GetAllGenresAsync()).ReturnsAsync(genres);
            _authorServiceMock.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(authors);

            var editBookViewModel = new EditBookViewModel
            {
                BookId = bookId,
                Title = "Updated Book",
                Description = "Updated Description",
                PublicationDate = DateTime.Now,
                AvailabilityStatus = AvailabilityStatus.Available,
                SelectedGenre = "Invalid Genre", // Invalid genre
                SelectedAuthors = new List<string> { "Author 1" }
            };

            // Act
            var result = await _controller.EditBook(editBookViewModel) as ViewResult;
            var model = result?.Model as EditBookViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(editBookViewModel.BookId, model?.BookId);
            Assert.Equal(editBookViewModel.Title, model?.Title);
            Assert.Equal(editBookViewModel.Description, model?.Description);
            Assert.Equal(editBookViewModel.PublicationDate, model?.PublicationDate);
            Assert.Equal(editBookViewModel.AvailabilityStatus, model?.AvailabilityStatus);
            Assert.Equal(editBookViewModel.SelectedGenre, model?.SelectedGenre);
            Assert.Equal(editBookViewModel.SelectedAuthors, model?.SelectedAuthors);
        }

        [Fact]
        public async Task DeleteBook_ExistingBookId_DeletesBookAndRedirectsToSearch()
        {
            // Arrange
            var bookId = 1;

            _bookServiceMock.Setup(x => x.DeleteBookAsync(bookId)).Verifiable();

            // Act
            var result = await _controller.DeleteBook(bookId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Search", result.ActionName);
        }

        [Fact]
        public async Task CreateBook_ReturnsViewResultWithCreateBookViewModel()
        {
            // Arrange
            var genres = new List<GenreDTO> { new GenreDTO { GenreId = 1, Name = "Genre 1" }, new GenreDTO { GenreId = 2, Name = "Genre 2" } };
            var authors = new List<AuthorDTO> { new AuthorDTO { AuthorId = 1, AuthorName = "Author 1" } };

            _genreServiceMock.Setup(x => x.GetAllGenresAsync()).ReturnsAsync(genres);
            _authorServiceMock.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(authors);

            var expectedViewModel = new CreateBookViewModel
            {
                Genres = genres.ToList(),
                Authors = authors.ToList()
            };

            // Act
            var result = await _controller.CreateBook() as ViewResult;
            var model = result?.Model as CreateBookViewModel;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedViewModel.Genres.Count, model?.Genres.Count);
            Assert.Equal(expectedViewModel.Authors.Count, model?.Authors.Count);
        }
    }
}
