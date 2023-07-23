using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.RepositoriesInterfaces
{
    public interface IReviewRepository
    {
        Task<ICollection<Review>> GetReviewsByBookIdAsync(int bookId);
        Task<double> GetAverageRatingForBookAsync(int bookId);
        Task CreateReviewAsync(Review review);
    }
}
