using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
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

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByBookIdAsync(int bookId)
        {
            return _mapper.Map<IEnumerable<ReviewDTO>>(await _reviewRepository.GetReviewsByBookIdAsync(bookId));
        }

        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            return await _reviewRepository.GetAverageRatingForBookAsync(bookId);
        }

        public async Task CreateReviewAsync(ReviewDTO reviewDTO)
        {
            var review = _mapper.Map<Review>(reviewDTO);

            await _reviewRepository.CreateReviewAsync(review);
        }
    }
}
