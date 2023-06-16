using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author> GetAuthorByIdAsync(int authorId);

        Task<bool> CreateAuthorAsync(Author author);
        Task<Author> UpdateAuthorAsync(int authorId, Author authorDTO);
        Task<bool> DeleteAuthorAsync(int authorId);
    }
}
