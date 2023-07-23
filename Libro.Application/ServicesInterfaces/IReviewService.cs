using Libro.Application.DTOs;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsByBookIdAsync(int bookId);
        Task<double> GetAverageRatingForBookAsync(int bookId);
        Task CreateReviewAsync(ReviewDTO reviewDTO);
    }
}
