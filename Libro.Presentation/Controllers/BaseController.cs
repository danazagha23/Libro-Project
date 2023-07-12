using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Libro.Presentation.Controllers
{
    public class BaseController : Controller
    {
        private readonly INotificationService _notificationService;

        public BaseController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int unreadNotificationCount = 0;
            var notifications = new List<NotificationDTO>();

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                notifications = _notificationService.GetNotificationsForUserAsync(userId).Result;
            }

            var layoutViewModel = new LayoutViewModel
            {
                UnreadNotificationCount = unreadNotificationCount,
                Notifications = notifications
            };

            ViewData["LayoutViewModel"] = layoutViewModel;

            base.OnActionExecuting(context);
        }
    }
}
