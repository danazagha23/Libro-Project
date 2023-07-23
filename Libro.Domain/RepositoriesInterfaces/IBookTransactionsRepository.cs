using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.RepositoriesInterfaces
{
    public interface IBookTransactionsRepository
    {
        Task<bool> CreateTransactionAsync(BookTransaction transaction);
        Task<ICollection<BookTransaction>> GetAllBookTransactionsAsync();
        Task<BookTransaction> GetTransactionByIdAsync(int transactionId);
        Task<ICollection<BookTransaction>> GetTransactionsByUserIdAsync(int userId);
        Task<ICollection<BookTransaction>> GetTransactionsByBookIdAsync(int bookId);
        Task<BookTransaction> UpdateTransactionAsync(int transactionId, BookTransaction transaction);
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task<ICollection<BookTransaction>> FindTransactionsAsync(string selectedType, string SelectedPatron, string SelectedBook);
    }
}
