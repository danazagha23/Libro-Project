using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDTO User { get; set; }
    }
}
