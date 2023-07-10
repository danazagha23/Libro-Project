using Libro.Domain.Entities;
using Libro.Domain.Enums;

namespace Libro.Application.DTOs
{
    public class UserDTO
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

        public IEnumerable<ReadingListDTO> ReadingLists { get; set; }
        public IEnumerable<BookTransactionDTO> BookTransactions { get; set; }
    }
}