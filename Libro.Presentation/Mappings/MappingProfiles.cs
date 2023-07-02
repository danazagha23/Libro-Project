using AutoMapper;
using Libro.Application.DTOs;
using Libro.Presentation.Models;

namespace Libro.Presentation.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<RegisterViewModel, UserDTO>().ReverseMap();
            CreateMap<UserProfileViewModel, UserDTO>().ReverseMap();
            CreateMap<BookDetailsViewModel, BookDTO>().ReverseMap();
            CreateMap<EditUserViewModel, UserDTO>().ReverseMap();
        }
    }
}
