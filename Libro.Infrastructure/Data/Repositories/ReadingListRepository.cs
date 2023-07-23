using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ReadingListRepository> _logger;

        public ReadingListRepository(LibroDbContext dbContext, ILogger<ReadingListRepository> logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        public async Task<ReadingList> GetReadingListByIdAsync(int readingListId)
        {
            try
            {
                _logger.LogInformation("Fetching reading list by ID: {ReadingListId} from the database.", readingListId);
                var readingList = await _context.ReadingLists
                    .Include(rl => rl.Books)
                    .FirstOrDefaultAsync(rl => rl.Id == readingListId);

                return readingList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching reading list by ID: {ReadingListId}.", readingListId);
                throw;
            }
        }

        public async Task<ICollection<ReadingList>> GetReadingListsByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching reading lists for user ID: {UserId} from the database.", userId);
                var readingLists = await _context.ReadingLists
                    .Include(rl => rl.Books)
                    .Where(rl => rl.UserId == userId)
                    .ToListAsync();

                return readingLists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching reading lists for user ID: {UserId}.", userId);
                throw;
            }
        }

        public async Task<ReadingList> CreateReadingListAsync(ReadingList readingList)
        {
            try
            {
                _logger.LogInformation("Creating a new reading list in the database.");
                _context.ReadingLists.Add(readingList);
                await _context.SaveChangesAsync();

                return readingList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new reading list.");
                throw;
            }
        }

        public async Task<bool> AddBookToReadingListAsync(int readingListId, int bookId)
        {
            try
            {
                _logger.LogInformation("Adding book with ID: {BookId} to reading list with ID: {ReadingListId} in the database.", bookId, readingListId);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book with ID: {BookId} to reading list with ID: {ReadingListId}.", bookId, readingListId);
                throw;
            }
        }

        public async Task<bool> RemoveBookFromReadingListAsync(int readingListId, int bookId)
        {
            try
            {
                _logger.LogInformation("Removing book with ID: {BookId} from reading list with ID: {ReadingListId} in the database.", bookId, readingListId);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing book with ID: {BookId} from reading list with ID: {ReadingListId}.", bookId, readingListId);
                throw;
            }
        }
    }
}
