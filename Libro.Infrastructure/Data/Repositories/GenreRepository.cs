using Libro.Domain.Entities;
using Libro.Domain.Interfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly LibroDbContext _context;
        public GenreRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId)
        {
            return await _context.Books.Where(id => id.GenreId == genreId).ToListAsync();
        }

    }
}
