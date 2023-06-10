using Libro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class BookTransaction
    {
        public int TransactionId { get; set; }
        public int BookId { get; set; }
        public int PatronId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime DueDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public bool IsReturned { get; set; }

        public Book Book { get; set; }
        public User Patron { get; set; }
    }
}
