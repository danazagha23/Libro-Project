using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.DTOs
{
    public class AuthorDTO
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string? Biography { get; set; }

        public IEnumerable<BookDTO> Books { get; set; }
    }
}
