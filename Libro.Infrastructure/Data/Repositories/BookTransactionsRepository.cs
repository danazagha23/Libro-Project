using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
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
        public BookTransactionsRepository(LibroDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<bool> CreateTransactionAsync(BookTransaction transaction)
        {
            _context.BookTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<BookTransaction>> GetAllBookTransactionsAsync()
        {
            return await _context.BookTransactions
                .Include(book => book.Book)
                .Include(user => user.Patron)
                .ToListAsync();
        }    
        public async Task<BookTransaction> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.BookTransactions.FindAsync(transactionId);
        }
        public async Task<List<BookTransaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.BookTransactions
                .Where(t => t.PatronId == userId)
                .ToListAsync();
        }
        public async Task<List<BookTransaction>> GetTransactionsByBookIdAsync(int bookId)
        {
            return await _context.BookTransactions
                .Where(t => t.BookId == bookId)
                .ToListAsync();
        }

        public async Task<BookTransaction> UpdateTransactionAsync(int transactionId, BookTransaction transaction)
        {
            var existingTransaction = await _context.BookTransactions.FindAsync(transactionId);
            if (existingTransaction == null)
            {
                throw new Exception("Transaction not Found");
            }
            else
            {
                _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
                await _context.SaveChangesAsync();
            }
            return existingTransaction;
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            var transaction = await _context.BookTransactions.FindAsync(transactionId);
            if (transaction == null)
            {
                throw new Exception("Book not Found");
            }
            else
            {
                _context.BookTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return true;
        }


        public async Task<List<BookTransaction>> FindTransactionsAsync(string selectedType, string SelectedPatron, string SelectedBook, string Status)
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
    }
}
