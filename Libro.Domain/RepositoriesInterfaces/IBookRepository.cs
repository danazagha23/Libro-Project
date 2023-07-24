using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.RepositoriesInterfaces
{
    public interface IBookRepository
    {
        Task<ICollection<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(int bookId);

        Task<Book> CreateBookAsync(Book book);
        Task<BookAuthor> CreateBookAuthorAsync(int bookId, int authorId);
        Task<Book> UpdateBookAsync(int bookId, Book book);
        Task<bool> DeleteBookAsync(int bookId);
        Task<bool> DeleteBookAuthorsByBookIdAsync(int bookId);

        Task<ICollection<Book>> FindBooksAsync(string bookGenre, string searchString, string authorName, string availabilityStatus);
    }
}
