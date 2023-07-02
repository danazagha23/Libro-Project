using Libro.Application.DTOs;

namespace Libro.Presentation.Models
{
    public class UsersViewModel
    {
        public List<UserDTO> Users { get; set; }     
        public List<UserDTO>? FilteredUsers { get; set; }     
        public string SelectedUser { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
