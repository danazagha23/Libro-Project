using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Libro.Presentation.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IUserManagementService _userManagementService;

        public NotificationController(INotificationService notificationService, IUserManagementService userManagementService)
        {
            _notificationService = notificationService;
            _userManagementService = userManagementService;
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> Notification()
        {
            return View();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> SendDueDateNotification(string username, DateTime dueDate)
        {
            var message = $"Your book is due on {dueDate}. Please return it on time.";
            var user = await _userManagementService.GetUserByUsernameAsync(username);   
            await _notificationService.CreateNotificationAsync(user.UserId, message);

            return RedirectToAction("Notification");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> SendReservationNotification(string username, BookDTO reservedBook)
        {
            var message = $"The book '{reservedBook.Title}' is now available for pickup.";
            var user = await _userManagementService.GetUserByUsernameAsync(username);
            await _notificationService.CreateNotificationAsync(user.UserId, message);

            return RedirectToAction("Notification");
        }

    }
}
