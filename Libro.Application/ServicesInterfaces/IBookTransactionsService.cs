using Libro.Application.DTOs;
using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IBookTransactionsService
    {
        Task<bool> CreateTransactionAsync(BookTransactionDTO transactionDTO);
        Task<List<BookTransactionDTO>> GetAllBookTransactionsAsync();
        Task<BookTransactionDTO> GetTransactionByIdAsync(int transactionId);
        Task<List<BookTransactionDTO>> GetTransactionsByUserIdAsync(int userId);
        Task<List<BookTransactionDTO>> GetTransactionsByBookIdAsync(int bookId);
        Task<BookTransactionDTO> UpdateTransactionAsync(int transactionId, BookTransactionDTO transactionDTO);
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task ReserveBookAsync(int bookId, int patronId);
        Task CheckOutBookAsync(int transactionId);
        Task AcceptReturnAsync(int transactionId);
        Task<List<BookTransactionDTO>> FindTransactionsAsync(string selectedType, string SelectedPatron, string SelectedBook, string Status);
    }
}
