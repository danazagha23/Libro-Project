using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Libro.Presentation.Models
{
    public class SearchViewModel
    {
        public List<BookDTO>? AllBooks { get; set; }
        public List<BookDTO>? FilteredBooks { get; set; }
        public List<string>? Genres { get; set; }
        public string? BookGenre { get; set; }
        public List<AuthorDTO>? AllAuthors { get; set; }
        public string? AuthorName { get; set; }
        public string? SearchString { get; set; }
        public string? AvailabilityStatus { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
