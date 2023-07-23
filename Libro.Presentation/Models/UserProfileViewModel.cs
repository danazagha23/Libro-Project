using Libro.Domain.Enums;

namespace Libro.Presentation.Models
{
    public class UserProfileViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public UserRole Role { get; set; }
    }
}
