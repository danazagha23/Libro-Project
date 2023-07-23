using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class BookTransactionsService : IBookTransactionsService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookTransactionsRepository _transactionsRepository;
        private readonly IMapper _mapper;

        public BookTransactionsService(IBookRepository bookRepository, IUserRepository userRepository, IBookTransactionsRepository transactionsRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _transactionsRepository = transactionsRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateTransactionAsync(BookTransactionDTO transactionDTO)
        {
            var transaction = _mapper.Map<BookTransaction>(transactionDTO);
            await _transactionsRepository.CreateTransactionAsync(transaction);

            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            return await _transactionsRepository.DeleteTransactionAsync(transactionId);
        }

        public async Task<ICollection<BookTransactionDTO>> GetAllBookTransactionsAsync()
        {
            var transactions = await _transactionsRepository.GetAllBookTransactionsAsync();
            var transactionsDTO = _mapper.Map<IEnumerable<BookTransactionDTO>>(transactions);

            return transactionsDTO.ToList();
        } 

        public async Task ReserveBookAsync(int bookId, int patronId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            var patron = await _userRepository.GetUserByIdAsync(patronId);

            if (book.AvailabilityStatus != AvailabilityStatus.Available)
            {
                throw new Exception("This book is not available for borrowing");
            }

            BookTransaction bookTransaction = new BookTransaction
            {
                PatronId = patronId,
                BookId = bookId,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Reserved,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false
            };

            await _transactionsRepository.CreateTransactionAsync(bookTransaction);

            book.AvailabilityStatus = AvailabilityStatus.Reserved;
            await _bookRepository.UpdateBookAsync(bookId, book);
        }

        public async Task CheckOutBookAsync(int transactionId)
        {
            var transaction = await _transactionsRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
            {
                throw new Exception("Book transaction not found");
            }

            if (transaction.TransactionType != TransactionType.Reserved || transaction.IsReturned)
            {
                throw new Exception("Invalid book transaction for check-out");
            }

            transaction.TransactionType = TransactionType.Borrowed;
            transaction.TransactionDate = DateTime.Now;
            transaction.DueDate = DateTime.Now.AddDays(14);

            var book = await _bookRepository.GetBookByIdAsync(transaction.BookId);
            if (book == null)
            {
                throw new Exception("Book not found");
            }
            book.AvailabilityStatus = AvailabilityStatus.Borrowed;

            await _transactionsRepository.UpdateTransactionAsync(transactionId, transaction);
            await _bookRepository.UpdateBookAsync(book.BookId, book);
        }

        public async Task AcceptReturnAsync(int transactionId)
        {
            var transaction = await _transactionsRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
            {
                throw new Exception("Book transaction not found");
            }

            if (transaction.TransactionType != TransactionType.Borrowed || transaction.IsReturned)
            {
                throw new Exception("Invalid book transaction for return");
            }

            transaction.TransactionType = TransactionType.Returned;
            transaction.IsReturned = true;

            var book = await _bookRepository.GetBookByIdAsync(transaction.BookId);
            if (book == null)
            {
                throw new Exception("Book not found");
            }
            book.AvailabilityStatus = AvailabilityStatus.Available;

            await _transactionsRepository.UpdateTransactionAsync(transactionId, transaction);
            await _bookRepository.UpdateBookAsync(book.BookId, book);
        }

        public async Task<ICollection<BookTransactionDTO>> FindTransactionsAsync(string selectedType, string SelectedPatron, string SelectedBook)
        {
            var searchResults = await _transactionsRepository.FindTransactionsAsync(selectedType, SelectedPatron, SelectedBook);

            return _mapper.Map<ICollection<BookTransactionDTO>>(searchResults).ToList();
        }

    }
}
