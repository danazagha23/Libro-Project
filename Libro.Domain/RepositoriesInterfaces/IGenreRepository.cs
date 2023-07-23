using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.RepositoriesInterfaces
{
    public interface IGenreRepository
    {
        Task<ICollection<Genre>> GetAllGenresAsync();
        Task<ICollection<Book>> GetBooksByGenreAsync(int genreId);
    }
}
