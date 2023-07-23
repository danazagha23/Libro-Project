using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class PatronsViewModel
    {
        public List<UserDTO> Patrons { get; set; }     
        public List<UserDTO>? FilteredPatrons { get; set; }     
        public string SelectedPatron { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
