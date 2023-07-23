using Libro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IReadingListService
    {
        Task<ReadingListDTO> GetReadingListByIdAsync(int readingListId);
        Task<IEnumerable<ReadingListDTO>> GetReadingListsByUserIdAsync(int userId);
        Task<ReadingListDTO> CreateReadingListAsync(int userId);
        Task<bool> AddBookToReadingListAsync(int readingListId, int bookId);
        Task<bool> RemoveBookFromReadingListAsync(int readingListId, int bookId);
        Task<bool> IsBookInReadingListAsync(int readingListId, int bookId);
    }
}
