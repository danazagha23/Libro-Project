using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.RepositoriesInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class GenreManagementService : IGenreManagementService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenreManagementService> _logger;

        public GenreManagementService(IGenreRepository genreRepository, IMapper mapper, ILogger<GenreManagementService> logger)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ICollection<GenreDTO>> GetAllGenresAsync()
        {
            try
            {
                var genres = await _genreRepository.GetAllGenresAsync();
                var genresDTO = _mapper.Map<ICollection<GenreDTO>>(genres);

                return genresDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all genres.");
                throw;
            }
        }

        public async Task<ICollection<BookDTO>> GetBooksByGenreAsync(int genreId)
        {
            try
            {
                var books = await _genreRepository.GetBooksByGenreAsync(genreId);
                var booksDTO = _mapper.Map<ICollection<BookDTO>>(books);

                return booksDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting books by genre.");
                throw;
            }
        }
    }
}
