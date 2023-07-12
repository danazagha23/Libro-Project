using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Libro.Application.Services
{
    public class BookManagementService : IBookManagementService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public BookManagementService(IBookRepository bookRepository, IAuthorRepository authorRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            var booksDTO = _mapper.Map<IEnumerable<BookDTO>>(books);

            return booksDTO;
        }
        public async Task<BookDTO> GetBookByIdAsync(int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            var bookDTO = _mapper.Map<BookDTO>(book);

            return bookDTO;
        }
        public async Task<BookDTO> CreateBookAsync(BookDTO bookDTO)
        {
            var book = _mapper.Map<Book>(bookDTO);
            await _bookRepository.CreateBookAsync(book);

            return _mapper.Map<BookDTO>(book);
        }
        public async Task<BookAuthorDTO> CreateBookAuthorAsync(int bookId, int authorId)
        {
            var bookAuthor = await _bookRepository.CreateBookAuthorAsync(bookId, authorId);

            return _mapper.Map<BookAuthorDTO>(bookAuthor);
        }
        public async Task<BookDTO> UpdateBookAsync(int bookId, BookDTO bookDTO)
        {
            var book = _mapper.Map<Book>(bookDTO);
            await _bookRepository.UpdateBookAsync(bookId, book);

            return _mapper.Map<BookDTO>(book);
        }
        public async Task<bool> DeleteBookAsync(int bookId)
        {
            return await _bookRepository.DeleteBookAsync(bookId);
        }
        public async Task<bool> DeleteBookAuthorsByBookIdAsync(int bookId)
        {
            return await _bookRepository.DeleteBookAuthorsByBookIdAsync(bookId);
        }

        public async Task<List<BookDTO>> FindBooksAsync(string bookGenre, string searchString, string authorName, string availabilityStatus)
        {
            var searchResults = await _bookRepository.FindBooksAsync(bookGenre, searchString, authorName, availabilityStatus);

            return _mapper.Map<IEnumerable<BookDTO>>(searchResults).ToList();
        }
    }
}
