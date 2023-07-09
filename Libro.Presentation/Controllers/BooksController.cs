using Auth0.ManagementApi.Paging;
using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Libro.Presentation.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookManagementService _bookManagementService;
        private readonly IGenreManagementService _genreManagementService;
        private readonly IAuthorManagementService _authorManagementService;
        private readonly IMapper _mapper;

        public BooksController(IBookManagementService bookManagementService, IGenreManagementService genreManagementService,IAuthorManagementService authorManagementService, IMapper mapper)
        {
            _bookManagementService = bookManagementService;
            _genreManagementService = genreManagementService;
            _authorManagementService = authorManagementService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string bookGenre, string searchString, string authorName, string availabilityStatus, int page = 1, int pageSize = 5)
        {
            var allBooks = await _bookManagementService.GetAllBooksAsync();
            var searchResults = await _bookManagementService.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus);
            var authors = await _authorManagementService.GetAllAuthorsAsync();
            var genres = await _genreManagementService.GetAllGenresAsync();
            var genreNames = genres.Select(g => g.Name).ToList();

            var pagedAllBooks = allBooks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pagedFilteredBooks = searchResults.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var searchViewModel = new SearchViewModel
            {
                Genres = genreNames,
                AllBooks = pagedAllBooks,
                FilteredBooks = pagedFilteredBooks,
                BookGenre = bookGenre,
                AllAuthors = authors.ToList(),
                SearchString = searchString,
                AuthorName = authorName,
                AvailabilityStatus = availabilityStatus,

                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)searchResults.Count() / pageSize)
            };

            return View(searchViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> BookDetails(int id)
        {
            var book = await _bookManagementService.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound(); 
            }

            var detailsViewModel = _mapper.Map<BookDetailsViewModel>(book);
            detailsViewModel.Genre = book.Genre;
            return View(detailsViewModel);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> EditBook(int bookId)
        {
            var book = await _bookManagementService.GetBookByIdAsync(bookId);
            var Genres = await _genreManagementService.GetAllGenresAsync();
            var authors = await _authorManagementService.GetAllAuthorsAsync();

            if (book == null)
            {
                return NotFound();
            }

            var bookViewModel = new EditBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                AvailabilityStatus = book.AvailabilityStatus,
                AllGenres = Genres.ToList(),
                SelectedGenre = book.Genre.Name,
                AllAuthors = authors.ToList(),
                SelectedAuthors = book.BookAuthors?.Select(a => a.Author.AuthorName)?.ToList() ?? new List<string>()
            };

            return View(bookViewModel);
        }


        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> EditBook(EditBookViewModel bookDetailsModel)
        {
            try
            {
                var book = await _bookManagementService.GetBookByIdAsync(bookDetailsModel.BookId);
                var Genres = await _genreManagementService.GetAllGenresAsync();
                bookDetailsModel.AllGenres = Genres.ToList();

                var selectedGenre = bookDetailsModel.AllGenres.Find(g => g.Name == bookDetailsModel.SelectedGenre);

                book.Title = bookDetailsModel.Title;
                book.Description = bookDetailsModel.Description;
                book.PublicationDate = bookDetailsModel.PublicationDate;
                book.AvailabilityStatus = bookDetailsModel.AvailabilityStatus;
                book.GenreId = selectedGenre.GenreId;
                book.Genre = selectedGenre;

                var selectedAuthors = bookDetailsModel.SelectedAuthors;

                var existingAuthors = book.BookAuthors.Select(ba => ba.Author.AuthorName).ToList();

                if (!selectedAuthors.SequenceEqual(existingAuthors))
                {
                    await _bookManagementService.DeleteBookAuthorsByBookIdAsync(book.BookId);

                    foreach (var authorName in bookDetailsModel.SelectedAuthors)
                    {
                        var author = await _authorManagementService.GetAuthorByNameAsync(authorName);
                        await _bookManagementService.CreateBookAuthorAsync(book.BookId, author.AuthorId);
                    }
                }

                await _bookManagementService.UpdateBookAsync(book.BookId, book);

                return RedirectToAction("Search");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(bookDetailsModel);
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            try
            {
                await _bookManagementService.DeleteBookAsync(bookId);
                return RedirectToAction("Search");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> CreateBook()
        {
            var genres = await _genreManagementService.GetAllGenresAsync();
            var authors = await _authorManagementService.GetAllAuthorsAsync();

            var viewModel = new CreateBookViewModel
            {
                Genres = genres.ToList(),
                Authors = authors.ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> CreateBook(CreateBookViewModel bookViewModel)
        {
            var genres = await _genreManagementService.GetAllGenresAsync();
            bookViewModel.Genres = genres.ToList();
            var selectedGenre = bookViewModel.Genres.FirstOrDefault(genre => genre.Name == bookViewModel.SelectedGenre);

            var book = new BookDTO
            {
                Title = bookViewModel.Title,
                Description = bookViewModel.Description,
                PublicationDate = bookViewModel.PublicationDate,
                AvailabilityStatus = bookViewModel.AvailabilityStatus,
                GenreId = selectedGenre.GenreId
            };

            var createdBook = await _bookManagementService.CreateBookAsync(book);
            createdBook = await _bookManagementService.GetBookByIdAsync(createdBook.BookId);

            foreach (var authorName in bookViewModel.SelectedAuthors)
            {
                var author = await _authorManagementService.GetAuthorByNameAsync(authorName);
                await _bookManagementService.CreateBookAuthorAsync(createdBook.BookId, author.AuthorId);
            }
            return Redirect("Search");
        }

    }
}
