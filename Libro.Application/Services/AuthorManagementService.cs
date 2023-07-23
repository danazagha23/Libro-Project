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
    public class AuthorManagementService : IAuthorManagementService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        public AuthorManagementService(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }
        public async Task<ICollection<AuthorDTO>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            var authorsDTO = _mapper.Map<ICollection<AuthorDTO>>(authors);

            return authorsDTO;
        }
        public async Task<AuthorDTO> GetAuthorByIdAsync(int authorId)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(authorId);
            var authorDTO = _mapper.Map<AuthorDTO>(author);

            return authorDTO;
        }       
        public async Task<AuthorDTO> GetAuthorByNameAsync(string authorName)
        {
            var author = await _authorRepository.GetAuthorByNameAsync(authorName);
            var authorDTO = _mapper.Map<AuthorDTO>(author);

            return authorDTO;
        }
        public async Task<AuthorDTO> CreateAuthorAsync(AuthorDTO authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);
            await _authorRepository.CreateAuthorAsync(author);

            return _mapper.Map<AuthorDTO>(author);
        }
        public async Task<AuthorDTO> UpdateAuthorAsync(int authorId, AuthorDTO authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);
            await _authorRepository.UpdateAuthorAsync(authorId, author);

            return _mapper.Map<AuthorDTO>(author);
        }
        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            return await _authorRepository.DeleteAuthorAsync(authorId);
        }
    }
}
