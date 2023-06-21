using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int bookId);

        Task<bool> CreateBookAsync(Book book);
        Task<Book> UpdateBookAsync(int bookId, Book book);
        Task<bool> DeleteBookAsync(int bookId);

        Task<List<Book>> FindBooksAsync(string bookGenre, string searchString, string authorName, string availabilityStatus);
    }
}
