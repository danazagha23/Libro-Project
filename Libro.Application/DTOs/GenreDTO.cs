using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.DTOs
{
    public class GenreDTO
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public IEnumerable<BookDTO> Books { get; set; }
    }
}
