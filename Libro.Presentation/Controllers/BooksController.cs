﻿using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Presentation.Helpers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookManagementService _bookManagementService;
        private readonly IGenreManagementService _genreManagementService;
        private readonly IAuthorManagementService _authorManagementService;
        private readonly IUserManagementService _userManagementService;
        private readonly IReadingListService _readingListService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly IPaginationWrapper<BookDTO> _paginationWrapper;

        public BooksController(IBookManagementService bookManagementService, IGenreManagementService genreManagementService,IAuthorManagementService authorManagementService, IUserManagementService userManagementService, IReadingListService readingListService, IMapper mapper, IReviewService reviewService, IPaginationWrapper<BookDTO> paginationWrapper)
        {
            _bookManagementService = bookManagementService;
            _genreManagementService = genreManagementService;
            _authorManagementService = authorManagementService;
            _userManagementService = userManagementService;
            _readingListService = readingListService;
            _mapper = mapper;
            _reviewService = reviewService;
            _paginationWrapper = paginationWrapper;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string bookGenre, string searchString, string authorName, string availabilityStatus, int page = 1, int pageSize = 5)
        {
            var allBooks = await _bookManagementService.GetAllBooksAsync();
            var searchResults = await _bookManagementService.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus);
            var authors = await _authorManagementService.GetAllAuthorsAsync();
            var genres = await _genreManagementService.GetAllGenresAsync();
            var genreNames = genres.Select(g => g.Name).ToList();

            var pagedFilteredBooks = _paginationWrapper.GetPage(searchResults, page, pageSize).ToList();

            var searchViewModel = new SearchViewModel
            {
                Genres = genreNames,
                AllBooks = allBooks.ToList(),
                FilteredBooks = pagedFilteredBooks,
                BookGenre = bookGenre,
                AllAuthors = authors.ToList(),
                SearchString = searchString,
                AuthorName = authorName,
                AvailabilityStatus = availabilityStatus,

                PageNumber = page,
                TotalPages = _paginationWrapper.GetTotalPages(searchResults.ToList(), pageSize)
            };

            return View(searchViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> BookDetails(int id)
        {
            var isBookInReadingList = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


                var readingLists = await _readingListService.GetReadingListsByUserIdAsync(userId);
                var readingList = readingLists.FirstOrDefault();

                
                if (readingList != null)
                {
                    isBookInReadingList = await _readingListService.IsBookInReadingListAsync(readingList.Id, id);
                }
            }

            var reviews = await _reviewService.GetReviewsByBookIdAsync(id);
            var averageRating = await _reviewService.GetAverageRatingForBookAsync(id);

            var book = await _bookManagementService.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            var detailsViewModel = _mapper.Map<BookDetailsViewModel>(book);
            detailsViewModel.Genre = book.Genre;
            detailsViewModel.IsBookInReadingList = isBookInReadingList;
            detailsViewModel.Reviews = reviews;
            detailsViewModel.averageRating = averageRating;

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
            try
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
                    GenreId = selectedGenre?.GenreId ?? 0
                };

                var createdBook = await _bookManagementService.CreateBookAsync(book);
                if (createdBook == null)
                {
                    throw new Exception("Failed to create the book.");
                }

                createdBook = await _bookManagementService.GetBookByIdAsync(createdBook.BookId);
                if (createdBook == null)
                {
                    throw new Exception("Failed to retrieve the created book.");
                }

                foreach (var authorName in bookViewModel.SelectedAuthors)
                {
                    var author = await _authorManagementService.GetAuthorByNameAsync(authorName);
                    if (author != null)
                    {
                        await _bookManagementService.CreateBookAuthorAsync(createdBook.BookId, author.AuthorId);
                    }
                }

                return RedirectToAction("Search");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(bookViewModel);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Patron")]
        public async Task<IActionResult> AddToReadingList(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var readingLists = await _readingListService.GetReadingListsByUserIdAsync(userId);
            var readingList = readingLists.FirstOrDefault();

            if (readingList == null)
            {
                var newReadingList = await _readingListService.CreateReadingListAsync(userId);
                readingLists.Add(newReadingList);
                readingList = newReadingList;
            }

            var isBookInReadingList = await _readingListService.IsBookInReadingListAsync(readingList.Id, id);

            if (!isBookInReadingList)
            {
                await _readingListService.AddBookToReadingListAsync(readingList.Id, id);
            }

            return RedirectToAction("BookDetails", "Books", new { id = id });
        }


        [HttpPost]
        [Authorize(Roles = "Patron")]
        public async Task<IActionResult> RemoveFromReadingList(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var readingLists = await _readingListService.GetReadingListsByUserIdAsync(userId);
            var readingList = readingLists.FirstOrDefault();

            await _readingListService.RemoveBookFromReadingListAsync(readingList.Id, id);

            return RedirectToAction("BookDetails", "Books", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int bookId, int rating, string comment)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userManagementService.GetUserByIdAsync(userId);

            // Get the book
            var book = await _bookManagementService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                return NotFound();
            }

            // Create the review
            var review = new ReviewDTO
            {
                UserId = user.UserId,
                BookId = book.BookId,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.Now
            };

            await _reviewService.CreateReviewAsync(review);
            
            return RedirectToAction("BookDetails", new { id = bookId });
        }

    }
}
