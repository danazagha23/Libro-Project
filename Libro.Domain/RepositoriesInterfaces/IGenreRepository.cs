using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId);
    }
}
