using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.RepositoriesInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class ReadingListService : IReadingListService
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly IMapper _mapper;

        public ReadingListService(IReadingListRepository readingListRepository, IMapper mapper)
        {
            _readingListRepository = readingListRepository;
            _mapper = mapper;
        }

        public async Task<ReadingListDTO> GetReadingListByIdAsync(int readingListId)
        {
            var readingList = await _readingListRepository.GetReadingListByIdAsync(readingListId);
            return _mapper.Map<ReadingListDTO>(readingList);
        }

        public async Task<ICollection<ReadingListDTO>> GetReadingListsByUserIdAsync(int userId)
        {
            var readingLists = await _readingListRepository.GetReadingListsByUserIdAsync(userId);
            return _mapper.Map<ICollection<ReadingListDTO>>(readingLists);
        }

        public async Task<ReadingListDTO> CreateReadingListAsync(int userId)
        {
            var readingList = new ReadingList { UserId = userId };
            var createdReadingList = await _readingListRepository.CreateReadingListAsync(readingList);
            return _mapper.Map<ReadingListDTO>(readingList); 
        }

        public async Task<bool> AddBookToReadingListAsync(int readingListId, int bookId)
        {
            return await _readingListRepository.AddBookToReadingListAsync(readingListId, bookId);
        }

        public async Task<bool> RemoveBookFromReadingListAsync(int readingListId, int bookId)
        {
            return await _readingListRepository.RemoveBookFromReadingListAsync(readingListId, bookId);
        }

        public async Task<bool> IsBookInReadingListAsync(int readingListId, int bookId)
        {
            var readingList = await _readingListRepository.GetReadingListByIdAsync(readingListId);
            if (readingList != null)
            {
                return readingList.Books.Any(b => b.BookId == bookId);
            }
            return false;
        }

    }
}
