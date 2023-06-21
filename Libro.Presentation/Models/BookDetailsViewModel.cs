using Libro.Application.DTOs;
using Libro.Domain.Enums;

namespace Libro.Presentation.Models
{
    public class BookDetailsViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int GenreId { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }

        public IEnumerable<BookAuthorDTO> BookAuthors { get; set; }
        public GenreDTO Genre { get; set; }
    }
}
