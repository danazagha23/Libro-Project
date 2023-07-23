using Libro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IAuthorManagementService
    {
        Task<ICollection<AuthorDTO>> GetAllAuthorsAsync();
        Task<AuthorDTO> GetAuthorByIdAsync(int authorId);
        Task<AuthorDTO> GetAuthorByNameAsync(string authorName);
        Task<AuthorDTO> CreateAuthorAsync(AuthorDTO authorDTO);
        Task<AuthorDTO> UpdateAuthorAsync(int authorId, AuthorDTO authorDTO);
        Task<bool> DeleteAuthorAsync(int authorId);
    }
}
