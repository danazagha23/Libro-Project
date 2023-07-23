using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class LayoutViewModel
    {
        public int UnreadNotificationCount { get; set; }
        public List<NotificationDTO> Notifications { get; set; }
    }
}
