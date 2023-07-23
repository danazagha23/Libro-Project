using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.DTOs
{
    public class BookAuthorDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        public BookDTO Book { get; set; }
        public AuthorDTO Author { get; set; }
    }
}
