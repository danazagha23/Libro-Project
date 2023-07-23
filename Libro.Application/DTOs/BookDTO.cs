using Libro.Domain.Entities;
using Libro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.DTOs
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int GenreId { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }

        public ICollection<BookAuthorDTO> BookAuthors { get; set; }
        public ICollection<ReadingListDTO> ReadingLists { get; set; }
        public GenreDTO Genre { get; set; }
    }
}
