using Auth0.ManagementApi.Paging;
using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> DetailsAsync(int id)
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

    }
}
