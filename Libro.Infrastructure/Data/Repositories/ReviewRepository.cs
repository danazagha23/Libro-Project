using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly LibroDbContext _context;

        public ReviewRepository(LibroDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(int bookId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            if (reviews.Count == 0)
            {
                return 0; 
            }

            double averageRating = reviews.Average(r => r.Rating);
            return averageRating;
        }


        public async Task CreateReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
    }
}
