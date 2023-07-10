using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class ReadingList
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
