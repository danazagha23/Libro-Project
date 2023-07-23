using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookManagementService _bookManagementService;
        private readonly INotificationService _notificationService;

        public HomeController(ILogger<HomeController> logger, IBookManagementService bookManagementService, INotificationService notificationService)
        {
            _logger = logger;
            _bookManagementService = bookManagementService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var books = await _bookManagementService.GetAllBooksAsync();
            var availableBooks = books.Where(b => b.AvailabilityStatus == AvailabilityStatus.Available);

            int unreadNotificationCount = 0;
            var notifications = new List<NotificationDTO>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                unreadNotificationCount = await _notificationService.GetUnreadNotificationCountAsync(unreadNotificationCount);
                notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            }

            var homeViewModel = new HomeViewModel
            {
                AvailableBooks = availableBooks,
                UnreadNotificationCount = unreadNotificationCount,
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