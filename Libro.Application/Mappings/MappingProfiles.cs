using AutoMapper;
using Libro.Application.DTOs;
using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<BookAuthor, BookAuthorDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<BookTransaction, BookTransactionDTO>().ReverseMap();
            CreateMap<ReadingList, ReadingListDTO>().ReverseMap();
        }
    }
}
