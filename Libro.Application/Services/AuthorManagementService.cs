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
    public class AuthorManagementService : IAuthorManagementService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorManagementService> _logger;

        public AuthorManagementService(IAuthorRepository authorRepository, IMapper mapper, ILogger<AuthorManagementService> logger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ICollection<AuthorDTO>> GetAllAuthorsAsync()
        {
            try
            {
                var authors = await _authorRepository.GetAllAuthorsAsync();
                var authorsDTO = _mapper.Map<ICollection<AuthorDTO>>(authors);

                return authorsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all authors.");
                throw;
            }
        }

        public async Task<AuthorDTO> GetAuthorByIdAsync(int authorId)
        {
            try
            {
                var author = await _authorRepository.GetAuthorByIdAsync(authorId);
                var authorDTO = _mapper.Map<AuthorDTO>(author);

                return authorDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting author with ID {authorId}.");
                throw;
            }
        }

        public async Task<AuthorDTO> GetAuthorByNameAsync(string authorName)
        {
            try
            {
                var author = await _authorRepository.GetAuthorByNameAsync(authorName);
                var authorDTO = _mapper.Map<AuthorDTO>(author);

                return authorDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting author with name '{authorName}'.");
                throw;
            }
        }

        public async Task<AuthorDTO> CreateAuthorAsync(AuthorDTO authorDTO)
        {
            try
            {
                var author = _mapper.Map<Author>(authorDTO);
                await _authorRepository.CreateAuthorAsync(author);

                return _mapper.Map<AuthorDTO>(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the author.");
                throw;
            }
        }

        public async Task<AuthorDTO> UpdateAuthorAsync(int authorId, AuthorDTO authorDTO)
        {
            try
            {
                var author = _mapper.Map<Author>(authorDTO);
                await _authorRepository.UpdateAuthorAsync(authorId, author);

                return _mapper.Map<AuthorDTO>(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating author with ID {authorId}.");
                throw;
            }
        }

        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            try
            {
                return await _authorRepository.DeleteAuthorAsync(authorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting author with ID {authorId}.");
                throw;
            }
        }
    }
}
