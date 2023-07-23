using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string? Biography { get; set; }

        public IEnumerable<Book> Books { get; set; }
    }
}
