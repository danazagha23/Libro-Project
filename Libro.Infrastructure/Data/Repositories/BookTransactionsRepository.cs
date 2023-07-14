using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class BookTransactionsRepository : IBookTransactionsRepository
    {
        private readonly LibroDbContext _context;
        private readonly ILogger<BookTransactionsRepository> _logger;

        public BookTransactionsRepository(LibroDbContext dbContext, ILogger<BookTransactionsRepository> logger)
        {
            _context = dbContext;
            _logger = logger;
        }

        public async Task<bool> CreateTransactionAsync(BookTransaction transaction)
        {
            try
            {
                _context.BookTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new book transaction.");
                throw;
            }
        }

        public async Task<List<BookTransaction>> GetAllBookTransactionsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all book transactions from the database.");
                return await _context.BookTransactions
                    .Include(book => book.Book)
                    .Include(user => user.Patron)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all book transactions.");
                throw;
            }
        }

        public async Task<BookTransaction> GetTransactionByIdAsync(int transactionId)
        {
            try
            {
                _logger.LogInformation("Fetching book transaction by ID: {TransactionId} from the database.", transactionId);
                return await _context.BookTransactions.FindAsync(transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book transaction by ID: {TransactionId}.", transactionId);
                throw;
            }
        }

        public async Task<List<BookTransaction>> GetTransactionsByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching book transactions by user ID: {UserId} from the database.", userId);
                return await _context.BookTransactions
                    .Where(t => t.PatronId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book transactions by user ID: {UserId}.", userId);
                throw;
            }
        }

        public async Task<List<BookTransaction>> GetTransactionsByBookIdAsync(int bookId)
        {
            try
            {
                _logger.LogInformation("Fetching book transactions by book ID: {BookId} from the database.", bookId);
                return await _context.BookTransactions
                    .Where(t => t.BookId == bookId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book transactions by book ID: {BookId}.", bookId);
                throw;
            }
        }

        public async Task<BookTransaction> UpdateTransactionAsync(int transactionId, BookTransaction transaction)
        {
            try
            {
                var existingTransaction = await _context.BookTransactions.FindAsync(transactionId);
                if (existingTransaction == null)
                {
                    throw new Exception("Transaction not found.");
                }

                _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
                await _context.SaveChangesAsync();

                return existingTransaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book transaction with ID: {TransactionId}.", transactionId);
                throw;
            }
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            try
            {
                var transaction = await _context.BookTransactions.FindAsync(transactionId);
                if (transaction == null)
                {
                    throw new Exception("Book transaction not found.");
                }

                _context.BookTransactions.Remove(transaction);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book transaction with ID: {TransactionId}.", transactionId);
                throw;
            }
        }

        public async Task<List<BookTransaction>> FindTransactionsAsync(string selectedType, string SelectedPatron, string SelectedBook, string Status)
        {
            try
            {
                var transactions = await GetAllBookTransactionsAsync();

                if (transactions == null)
                {
                    throw new Exception("Entity set Transactions is null.");
                }

                if (!string.IsNullOrEmpty(SelectedBook))
                {
                    transactions = transactions.Where(s => s.Book.Title!.Contains(SelectedBook)).ToList();
                }

                if (!string.IsNullOrEmpty(selectedType))
                {
                    transactions = transactions.Where(x => x.TransactionType.ToString() == selectedType).ToList();
                }

                if (!string.IsNullOrEmpty(SelectedPatron))
                {
                    transactions = transactions.Where(x => x.Patron.Username == SelectedPatron).ToList();
                }

                if (!string.IsNullOrEmpty(Status))
                {
                    bool isReturned = bool.Parse(Status);
                    transactions = transactions.Where(t => t.IsReturned == isReturned).ToList();
                }

                return transactions.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding book transactions.");
                throw;
            }
        }
    }
}
