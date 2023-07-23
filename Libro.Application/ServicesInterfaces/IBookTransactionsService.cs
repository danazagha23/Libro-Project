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
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task<ICollection<BookTransactionDTO>> GetAllBookTransactionsAsync();
        Task ReserveBookAsync(int bookId, int patronId);
        Task CheckOutBookAsync(int transactionId);
        Task AcceptReturnAsync(int transactionId);
        Task<ICollection<BookTransactionDTO>> FindTransactionsAsync(string selectedType, string SelectedPatron, string SelectedBook);
    }
}
