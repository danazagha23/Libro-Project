﻿using Libro.Application.DTOs;
using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IGenreManagementService
    {
        Task<ICollection<BookDTO>> GetBooksByGenreAsync(int genreId);
        Task<ICollection<GenreDTO>> GetAllGenresAsync();
    }
}
