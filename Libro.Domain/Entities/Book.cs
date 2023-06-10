using Libro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int GenreId { get; set; }
        public int AuthorId { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }

        public IEnumerable<Author> Authors { get; set; }
        public Genre Genre { get; set; }
    }
}
