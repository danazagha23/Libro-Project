using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class HomeViewModel
    {
        public IEnumerable<BookDTO> AvailableBooks { get; set; }
    }
}
