using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.Enums;

namespace Libro.Presentation.Models
{
    public class BookTransactionsViewModel
    {
        public IEnumerable<BookTransactionDTO> Transactions { get; set; }
        public IEnumerable<BookDTO> Books { get; set; }
        public List<BookTransactionDTO>? FilteredTransactions { get; set; }
        public string? SelectedType { get; set; }
        public List<UserDTO>? Patrons { get; set; }
        public string? SelectedPatron { get; set; }
        public string? SelectedBook { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }

}
