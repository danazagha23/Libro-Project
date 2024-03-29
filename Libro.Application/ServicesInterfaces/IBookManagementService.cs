﻿using Libro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IBookManagementService
    {
        Task<ICollection<BookDTO>> GetAllBooksAsync();
        Task<BookDTO> GetBookByIdAsync(int bookId);
        Task<BookDTO> CreateBookAsync(BookDTO bookDTO);
        Task<BookAuthorDTO> CreateBookAuthorAsync(int bookId, int authorId);
        Task<BookDTO> UpdateBookAsync(int bookId, BookDTO bookDTO);
        Task<bool> DeleteBookAsync(int bookId);
        Task<bool> DeleteBookAuthorsByBookIdAsync(int bookId);
        Task<ICollection<BookDTO>> FindBooksAsync(string bookGenre, string searchString, string authorName, string availabilityStatus);
    }
}
