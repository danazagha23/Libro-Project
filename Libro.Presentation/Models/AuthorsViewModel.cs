using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class AuthorsViewModel
    {
        public List<AuthorDTO> Authors { get; set; }
        public List<AuthorDTO>? FilteredAuthors { get; set; }
        public string SelectedAuthor { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
