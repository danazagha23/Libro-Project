using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class NotificationsViewModel
    {
        public List<NotificationDTO> Notifications { get; set;}
        public int UnreadNotificationCount { get; set;}
    }

}
