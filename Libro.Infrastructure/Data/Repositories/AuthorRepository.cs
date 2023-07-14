using Libro.Domain.Entities;
using Libro.Domain.Interfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<AuthorRepository> _logger;

        public AuthorRepository(LibroDbContext context, ILogger<AuthorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all authors from the database.");
                return await _context.Authors.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all authors.");
                throw;
            }
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            try
            {
                _logger.LogInformation("Fetching author by ID: {AuthorId} from the database.", authorId);
                var author = await _context.Authors.FindAsync(authorId);

                if (author == null)
                {
                    throw new Exception("Author not found.");
                }

                return author;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching author by ID: {AuthorId}.", authorId);
                throw;
            }
        }

        public async Task<Author> GetAuthorByNameAsync(string authorName)
        {
            try
            {
                _logger.LogInformation("Fetching author by name: {AuthorName} from the database.", authorName);
                var author = await _context.Authors.FirstOrDefaultAsync(a => a.AuthorName == authorName);

                if (author == null)
                {
                    throw new Exception("Author not found.");
                }

                return author;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching author by name: {AuthorName}.", authorName);
                throw;
            }
        }

        public async Task<bool> CreateAuthorAsync(Author author)
        {
            try
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new author.");
                throw;
            }
        }

        public async Task<Author> UpdateAuthorAsync(int authorId, Author author)
        {
            try
            {
                var existingAuthor = await _context.Authors.FindAsync(authorId);
                if (existingAuthor == null)
                {
                    throw new Exception("Author not found.");
                }

                _context.Entry(existingAuthor).CurrentValues.SetValues(author);
                await _context.SaveChangesAsync();

                return existingAuthor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating author with ID: {AuthorId}.", authorId);
                throw;
            }
        }

        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            try
            {
                var existingAuthor = await _context.Authors.FindAsync(authorId);
                if (existingAuthor == null)
                {
                    throw new Exception("Author not found.");
                }

                _context.Authors.Remove(existingAuthor);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting author with ID: {AuthorId}.", authorId);
                throw;
            }
        }
    }
}
