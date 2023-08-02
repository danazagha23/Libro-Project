using Libro.Application.DTOs;
using Libro.Application.Extensions;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(LibroDbContext context, ILogger<BookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ICollection<Book>> GetAllBooksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all books from the database.");
                var books = await _context.Books
                    .Include(book => book.BookAuthors)
                        .ThenInclude(bookAuthor => bookAuthor.Author)
                    .Include(book => book.Genre)
                    .Include(rl => rl.ReadingLists)
                    .Include(rl => rl.Reviews)
                    .ToListAsync();

                return books;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all books.");
                throw;
            }
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            try
            {
                _logger.LogInformation("Fetching book by ID: {BookId} from the database.", bookId);
                var book = await _context.Books
                    .Include(b => b.BookAuthors)
                        .ThenInclude(ba => ba.Author)
                    .Include(b => b.Genre)
                    .Include(rl => rl.ReadingLists)
                    .Include(rl => rl.Reviews)
                    .FirstOrDefaultAsync(b => b.BookId == bookId);

                if (book == null)
                {
                    throw new Exception("Book not found.");
                }

                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book by ID: {BookId}.", bookId);
                throw;
            }
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            try
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new book.");
                throw;
            }
        }

        public async Task<BookAuthor> CreateBookAuthorAsync(int bookId, int authorId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new book author.");
                throw;
            }
        }

        public async Task<Book> UpdateBookAsync(int bookId, Book book)
        {
            try
            {
                var existingBook = await _context.Books.FindAsync(bookId);
                if (existingBook == null)
                {
                    throw new Exception("Book not found.");
                }

                _context.Entry(existingBook).CurrentValues.SetValues(book);
                await _context.SaveChangesAsync();

                return existingBook;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with ID: {BookId}.", bookId);
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    throw new Exception("Book not found.");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book with ID: {BookId}.", bookId);
                throw;
            }
        }

        public async Task<bool> DeleteBookAuthorsByBookIdAsync(int bookId)
        {
            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    throw new Exception("Book not found.");
                }

                var bookAuthors = await _context.BookAuthors
                    .Where(ba => ba.BookId == bookId)
                    .ToListAsync();

                _context.BookAuthors.RemoveRange(bookAuthors);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book authors for book with ID: {BookId}.", bookId);
                throw;
            }
        }

        public async Task<ICollection<Book>> FindBooksAsync(string bookGenre, string searchString, string authorName, string availabilityStatus)
        {
            try
            {
                var books = await GetAllBooksAsync();

                if (books == null)
                {
                    throw new Exception("Entity set Books is null.");
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    books = books.Where(s => s.Title!.ContainsIgnoreCaseAndWhitespace(searchString)).ToList();
                }

                if (!string.IsNullOrEmpty(bookGenre))
                {
                    books = books.Where(x => x.Genre.Name == bookGenre).ToList();
                }

                if (!string.IsNullOrEmpty(authorName))
                {
                    books = books.Where(x => x.BookAuthors.Any(ab => ab.Author.AuthorName.ContainsIgnoreCaseAndWhitespace(authorName))).ToList();
                }

                if (!string.IsNullOrEmpty(availabilityStatus))
                {
                    books = books.Where(s => s.AvailabilityStatus.ToString() == availabilityStatus.ToString()).ToList();
                }

                return books.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding books.");
                throw;
            }
        }
    }
}
