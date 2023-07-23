using Libro.Application.DTOs;
using Libro.Domain.Enums;

namespace Libro.Presentation.Models
{
    public class EditBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }

        public string SelectedGenre { get; set; }
        public List<string> SelectedAuthors { get; set; }
        public List<GenreDTO> AllGenres { get; set; }
        public List<AuthorDTO> AllAuthors { get; set; }
    }
}


