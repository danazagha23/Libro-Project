using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class HomeViewModel
    {
        public IEnumerable<BookDTO> AvailableBooks { get; set; }
        public IEnumerable<BookDTO> BookRecommendations { get; set; }
        public int UnreadNotificationCount { get; set; }
        public List<NotificationDTO> Notifications { get; set; }
    }
}
