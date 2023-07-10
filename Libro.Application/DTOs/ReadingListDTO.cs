using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.DTOs
{
    public class ReadingListDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public UserDTO User { get; set; }
        public ICollection<BookDTO> Books { get; set; }
    }
}
