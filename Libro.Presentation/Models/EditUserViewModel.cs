using Libro.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Models
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public UserRole Role { get; set; }
    }
}
