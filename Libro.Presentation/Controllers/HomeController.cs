using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookManagementService _bookManagementService;
        private readonly IUserManagementService _userManagementService;
        private readonly INotificationService _notificationService;

        public HomeController(INotificationService notificationService, IUserManagementService userManagementService, IBookManagementService bookManagementService)
        {
            _userManagementService = userManagementService;
            _bookManagementService = bookManagementService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var books = await _bookManagementService.GetAllBooksAsync();
            var availableBooks = books.Where(b => b.AvailabilityStatus == AvailabilityStatus.Available);
            var notifications = new List<NotificationDTO>();

            IEnumerable<BookDTO> bookRecommendations = new List<BookDTO>();

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                bookRecommendations = await _userManagementService.GetUserRecommendationsAsync(userId);
                notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            }

            var homeViewModel = new HomeViewModel
            {
                AvailableBooks = availableBooks,
                BookRecommendations = bookRecommendations,
                UnreadNotificationCount = 0,
                Notifications = notifications
            };

            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
