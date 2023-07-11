using Libro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public UserRole Role { get; set; }

        public IEnumerable<ReadingList> ReadingLists { get; set; }
        public IEnumerable<Review> Notifications { get; set; }
        public IEnumerable<BookTransaction>? Transactions { get; set; }
    }
}
