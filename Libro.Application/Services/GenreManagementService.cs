using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.RepositoriesInterfaces;
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
        public GenreManagementService(IGenreRepository genreRepository, IMapper mapper) 
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }
        public async Task<ICollection<GenreDTO>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllGenresAsync();
            var genresDTO = _mapper.Map<ICollection<GenreDTO>>(genres);

            return genresDTO;
        }
        public async Task<ICollection<BookDTO>> GetBooksByGenreAsync(int genreId)
        {
            var books = await _genreRepository.GetBooksByGenreAsync(genreId);
            var booksDTO = _mapper.Map<ICollection<BookDTO>>(books);

            return booksDTO;
        }
    }
}
