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
    public class ReadingListService : IReadingListService
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReadingListService> _logger;

        public ReadingListService(IReadingListRepository readingListRepository, IMapper mapper, ILogger<ReadingListService> logger)
        {
            _readingListRepository = readingListRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReadingListDTO> GetReadingListByIdAsync(int readingListId)
        {
            try
            {
                var readingList = await _readingListRepository.GetReadingListByIdAsync(readingListId);
                return _mapper.Map<ReadingListDTO>(readingList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the reading list with ID {readingListId}.");
                throw;
            }
        }

        public async Task<ICollection<ReadingListDTO>> GetReadingListsByUserIdAsync(int userId)
        {
            try
            {
                var readingLists = await _readingListRepository.GetReadingListsByUserIdAsync(userId);
                return _mapper.Map<ICollection<ReadingListDTO>>(readingLists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting reading lists for user with ID {userId}.");
                throw;
            }
        }

        public async Task<ReadingListDTO> CreateReadingListAsync(int userId)
        {
            try
            {
                var readingList = new ReadingList { UserId = userId };
                var readingListCreated = await _readingListRepository.CreateReadingListAsync(readingList);

                return _mapper.Map<ReadingListDTO>(readingListCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating a reading list for user with ID {userId}.");
                throw;
            }
        }

        public async Task<bool> AddBookToReadingListAsync(int readingListId, int bookId)
        {
            try
            {
                return await _readingListRepository.AddBookToReadingListAsync(readingListId, bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding book with ID {bookId} to reading list with ID {readingListId}.");
                throw;
            }
        }

        public async Task<bool> RemoveBookFromReadingListAsync(int readingListId, int bookId)
        {
            try
            {
                return await _readingListRepository.RemoveBookFromReadingListAsync(readingListId, bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while removing book with ID {bookId} from reading list with ID {readingListId}.");
                throw;
            }
        }

        public async Task<bool> IsBookInReadingListAsync(int readingListId, int bookId)
        {
            try
            {
                var readingList = await _readingListRepository.GetReadingListByIdAsync(readingListId);
                if (readingList != null)
                {
                    return readingList.Books.Any(b => b.BookId == bookId);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while checking if book with ID {bookId} is in reading list with ID {readingListId}.");
                throw;
            }
        }
    }
}
