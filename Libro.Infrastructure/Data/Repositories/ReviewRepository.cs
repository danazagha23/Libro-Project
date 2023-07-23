using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Libro.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(LibroDbContext context, ILogger<ReviewRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ICollection<Review>> GetReviewsByBookIdAsync(int bookId)
        {
            try
            {
                _logger.LogInformation("Fetching reviews for book ID: {BookId} from the database.", bookId);
                return await _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.BookId == bookId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching reviews for book ID: {BookId}.", bookId);
                throw;
            }
        }

        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            try
            {
                _logger.LogInformation("Calculating average rating for book ID: {BookId} from the database.", bookId);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating average rating for book ID: {BookId}.", bookId);
                throw;
            }
        }

        public async Task CreateReviewAsync(Review review)
        {
            try
            {
                _logger.LogInformation("Creating a new review in the database for book ID: {BookId}.", review.BookId);
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new review for book ID: {BookId}.", review.BookId);
                throw;
            }
        }
    }
}
