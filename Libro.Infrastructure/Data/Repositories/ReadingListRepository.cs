using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class ReadingListRepository : IReadingListRepository
    {
        private readonly LibroDbContext _context;
        public ReadingListRepository(LibroDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<ReadingList> GetReadingListByIdAsync(int readingListId)
        {
            var readingList = await _context.ReadingLists
                .Include(rl => rl.Books)
                .FirstOrDefaultAsync(rl => rl.Id == readingListId);

            return readingList;
        }

        public async Task<IEnumerable<ReadingList>> GetReadingListsByUserIdAsync(int userId)
        {
            var readingLists = await _context.ReadingLists
                .Include(rl => rl.Books)
                .Where(rl => rl.UserId == userId)
                .ToListAsync();

            return readingLists;
        }

        public async Task<ReadingList> CreateReadingListAsync(ReadingList readingList)
        {
            _context.ReadingLists.Add(readingList);
            await _context.SaveChangesAsync();

            return readingList;
        }

        public async Task<bool> AddBookToReadingListAsync(int readingListId, int bookId)
        {
            var readingList = await _context.ReadingLists
                .Include(rl => rl.Books)
                .FirstOrDefaultAsync(rl => rl.Id == readingListId);

            var book = await _context.Books.FindAsync(bookId);

            if (readingList != null && book != null)
            {
                readingList.Books.Add(book);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveBookFromReadingListAsync(int readingListId, int bookId)
        {
            var readingList = await _context.ReadingLists
                .Include(rl => rl.Books)
                .FirstOrDefaultAsync(rl => rl.Id == readingListId);

            var book = readingList?.Books.FirstOrDefault(b => b.BookId == bookId);

            if (readingList != null && book != null)
            {
                readingList.Books.Remove(book);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
