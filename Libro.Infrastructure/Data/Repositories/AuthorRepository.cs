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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibroDbContext _context;
        public AuthorRepository(LibroDbContext context)
        {
            _context = context;
        } 

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                throw new Exception("Author not Found");
            }
            return author;
        }

        public async Task<bool> CreateAuthorAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Author> UpdateAuthorAsync(int authorId, Author author)
        {
            var existingAuthor = await _context.Authors.FindAsync(authorId);
            if (existingAuthor == null)
            {
                throw new Exception("Author not found");
            }
            else
            {
                _context.Entry(existingAuthor).CurrentValues.SetValues(author);
                await _context.SaveChangesAsync();
            }
            return existingAuthor;
        }

        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            var existingAuthor = await _context.Authors.FindAsync(authorId);
            if (existingAuthor == null)
            {
                throw new Exception("Author not Found");
            }
            else
            {
                _context.Authors.Remove(existingAuthor);
                await _context.SaveChangesAsync();
            }
            return true;
        }

    }
}
