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
    public class BookRepository : IBookRepository
    {
        private readonly LibroDbContext _context;
        public BookRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new Exception("Book not Found");
            }
            return book;
        }

        public async Task<bool> CreateBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return true;    
        }

        public async Task<Book> UpdateBookAsync(int bookId, Book book)
        {
            var existingBook = await _context.Books.FindAsync(bookId);
            if (existingBook == null)
            {
                throw new Exception("Book not Found");
            }
            else
            {
                _context.Entry(existingBook).CurrentValues.SetValues(book);
                await _context.SaveChangesAsync();
            }
            return book;
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new Exception("Book not Found");
            }
            else
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return true;
        }

    }
}
