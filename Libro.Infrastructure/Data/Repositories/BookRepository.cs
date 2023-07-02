using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
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
            var books = await _context.Books
                .Include(book => book.BookAuthors)
                    .ThenInclude(bookAuthor => bookAuthor.Author)
                .Include(book => book.Genre)
                .ToListAsync();

            return books;
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

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

        public async Task<BookAuthor> CreateBookAuthorAsync(int bookId, int authorId)
        {
            var bookAuthor = new BookAuthor
            {
                AuthorId = authorId,
                BookId = bookId
            };
            _context.BookAuthors.Add(bookAuthor);
            await _context.SaveChangesAsync();

            return bookAuthor;
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
        public async Task<bool> DeleteBookAuthorsByBookIdAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new Exception("Book not Found");
            }
            else
            {
                var bookAuthors = await _context.BookAuthors
                   .Where(ba => ba.BookId == bookId)
                   .ToListAsync();

                _context.BookAuthors.RemoveRange(bookAuthors);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<List<Book>> FindBooksAsync(string bookGenre, string searchString, string authorName, string availabilityStatus)
        {
            var books = await GetAllBooksAsync();

            if (books == null)
            {
                throw new Exception("Entity set Books is null.");
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title!.Contains(searchString)).ToList();
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                books = books.Where(x => x.Genre.Name == bookGenre).ToList();
            }

            if (!string.IsNullOrEmpty(authorName))
            {
                books = books.Where(x => x.BookAuthors.Any(ab => ab.Author.AuthorName.Contains(authorName))).ToList();
            }

            if (!string.IsNullOrEmpty(availabilityStatus))
            {
                books = books.Where(s => s.AvailabilityStatus.ToString() == availabilityStatus.ToString()).ToList();
            }

            return books.ToList();
        }

    }
}
