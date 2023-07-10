using Libro.Application.DTOs;
using Libro.Domain.Enums;

namespace Libro.Presentation.Models
{
    public class UserProfileViewModel
    {
        public UserDTO User { get; set; }
        public List<BookTransactionDTO> BorrowingHistory { get; set; }
        public List<BookTransactionDTO> CurrentLoans { get; set; }
        public List<BookTransactionDTO> OverdueLoans { get; set; }
        public List<ReadingListDTO> ReadingLists { get; set; }
    }
}
