using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ICollection<ReviewDTO>> GetReviewsByBookIdAsync(int bookId)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByBookIdAsync(bookId);
                return _mapper.Map<ICollection<ReviewDTO>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting reviews for book with ID {bookId}.");
                throw;
            }
        }

        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            try
            {
                return await _reviewRepository.GetAverageRatingForBookAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while calculating the average rating for book with ID {bookId}.");
                throw;
            }
        }

        public async Task CreateReviewAsync(ReviewDTO reviewDTO)
        {
            try
            {
                var review = _mapper.Map<Review>(reviewDTO);
                await _reviewRepository.CreateReviewAsync(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a review.");
                throw;
            }
        }
    }
}
