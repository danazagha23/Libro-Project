﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class BookAuthor
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        public Book Book { get; set; }
        public Author Author { get; set; }
    }
}
