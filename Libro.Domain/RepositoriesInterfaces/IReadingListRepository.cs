using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.RepositoriesInterfaces
{
    public interface IReadingListRepository
    {
        Task<ReadingList> GetReadingListByIdAsync(int readingListId);
        Task<ICollection<ReadingList>> GetReadingListsByUserIdAsync(int userId);
        Task<ReadingList> CreateReadingListAsync(ReadingList readingList);
        Task<bool> AddBookToReadingListAsync(int readingListId, int bookId);
        Task<bool> RemoveBookFromReadingListAsync(int readingListId, int bookId);
    }
}
