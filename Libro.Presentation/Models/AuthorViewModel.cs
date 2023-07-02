using Libro.Application.DTOs;
using Libro.Domain.Entities;

namespace Libro.Presentation.Models
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string? Biography { get; set; }
    }
}
