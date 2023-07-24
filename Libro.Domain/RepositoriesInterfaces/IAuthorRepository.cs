using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.RepositoriesInterfaces
{
    public interface IAuthorRepository
    {
        Task<ICollection<Author>> GetAllAuthorsAsync();
        Task<Author> GetAuthorByIdAsync(int authorId);
        Task<Author> GetAuthorByNameAsync(string authorName);

        Task<Author> CreateAuthorAsync(Author author);
        Task<Author> UpdateAuthorAsync(int authorId, Author authorDTO);
        Task<bool> DeleteAuthorAsync(int authorId);
    }
}
