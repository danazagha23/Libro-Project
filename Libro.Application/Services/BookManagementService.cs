using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class BookManagementService : IBookManagementService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookManagementService> _logger;

        public BookManagementService(IBookRepository bookRepository, ILogger<BookManagementService> logger, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ICollection<BookDTO>> GetAllBooksAsync()
        {
            try
            {
                var books = await _bookRepository.GetAllBooksAsync();
                var booksDTO = _mapper.Map<ICollection<BookDTO>>(books);
                return booksDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all books.");
                throw;
            }
        }

        public async Task<BookDTO> GetBookByIdAsync(int bookId)
        {
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(bookId);
                var bookDTO = _mapper.Map<BookDTO>(book);
                return bookDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting book with ID {bookId}.");
                throw;
            }
        }

        public async Task<BookDTO> CreateBookAsync(BookDTO bookDTO)
        {
            try
            {
                var book = _mapper.Map<Book>(bookDTO);
                await _bookRepository.CreateBookAsync(book);
                return _mapper.Map<BookDTO>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the book.");
                throw;
            }
        }

        public async Task<BookAuthorDTO> CreateBookAuthorAsync(int bookId, int authorId)
        {
            try
            {
                var bookAuthor = await _bookRepository.CreateBookAuthorAsync(bookId, authorId);
                return _mapper.Map<BookAuthorDTO>(bookAuthor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the book author.");
                throw;
            }
        }

        public async Task<BookDTO> UpdateBookAsync(int bookId, BookDTO bookDTO)
        {
            try
            {
                var book = _mapper.Map<Book>(bookDTO);
                await _bookRepository.UpdateBookAsync(bookId, book);
                return _mapper.Map<BookDTO>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating book with ID {bookId}.");
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            try
            {
                return await _bookRepository.DeleteBookAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting book with ID {bookId}.");
                throw;
            }
        }

        public async Task<bool> DeleteBookAuthorsByBookIdAsync(int bookId)
        {
            try
            {
                return await _bookRepository.DeleteBookAuthorsByBookIdAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting book authors for book with ID {bookId}.");
                throw;
            }
        }

        public async Task<ICollection<BookDTO>> FindBooksAsync(string bookGenre, string searchString, string authorName, string availabilityStatus)
        {
            try
            {
                var searchResults = await _bookRepository.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus);
                return _mapper.Map<ICollection<BookDTO>>(searchResults).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while finding books.");
                throw;
            }
        }
    }
}
