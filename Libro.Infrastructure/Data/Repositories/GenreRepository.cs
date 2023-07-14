using Libro.Domain.Entities;
using Libro.Domain.Interfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GenreRepository> _logger;

        public GenreRepository(LibroDbContext context, ILogger<GenreRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all genres from the database.");
                return await _context.Genres
                    .Include(genre => genre.Books)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all genres.");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId)
        {
            try
            {
                _logger.LogInformation("Fetching books by genre ID: {GenreId} from the database.", genreId);
                return await _context.Books
                    .Where(book => book.GenreId == genreId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books by genre ID: {GenreId}.", genreId);
                throw;
            }
        }
    }
}
