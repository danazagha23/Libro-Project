using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    public class BookTransactionsController : Controller
    {
        private readonly IBookTransactionsService _bookTransactionsService;
        private readonly IBookManagementService _bookManagementService;
        private readonly IUserManagementService _userManagementService;
        private readonly INotificationService _notificationService;

        public BookTransactionsController(IBookTransactionsService bookTransactionsService,IBookManagementService bookManagementService ,IUserManagementService userManagementService, INotificationService notificationService)
        {
            _bookTransactionsService = bookTransactionsService;
            _bookManagementService = bookManagementService;
            _userManagementService = userManagementService;
            _notificationService = notificationService;
        }

        [HttpPost]
        [Authorize(Roles="Patron")]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("User ID not found");
            }

            int patronId = int.Parse(userId);

            try
            {
                await _bookTransactionsService.ReserveBookAsync(bookId, patronId);
                //var successMessage = "Book reserved successfully";
                //TempData["SuccessMessage"] = successMessage;
                return RedirectToAction("BookDetails", "Books", new { id = bookId});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Index(string selectedType, string selectedPatron, string selectedBook, int page = 1, int pageSize = 5)
        {
            var bookTransactions = await _bookTransactionsService.GetAllBookTransactionsAsync();
            var searchResults = await _bookTransactionsService.FindTransactionsAsync(selectedType, selectedPatron, selectedBook);
            var users = await _userManagementService.GetAllUsersAsync();
            var patrons = users.ToList().Where(user => user.Role == UserRole.Patron);

            var books = await _bookManagementService.GetAllBooksAsync();
            var bookTitles = books.Select(b => b.Title).ToList();

            var pagedAllBooks = bookTransactions.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pagedFilteredBooks = searchResults.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var transactionsViewModel = new BookTransactionsViewModel
            {
                Transactions = pagedAllBooks,
                Books = books,
                FilteredTransactions = pagedFilteredBooks,
                Patrons = patrons.ToList(),
                SelectedBook = selectedBook,
                SelectedPatron = selectedPatron,
                SelectedType = selectedType,

                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)searchResults.Count() / pageSize)
            };

            return View(transactionsViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> CheckOut(int transactionId)
        {
            try
            {
                await _bookTransactionsService.CheckOutBookAsync(transactionId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AcceptReturn(int transactionId)
        {
            try
            {
                await _bookTransactionsService.AcceptReturnAsync(transactionId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
