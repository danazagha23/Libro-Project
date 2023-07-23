using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IValidationService
    {
        Task ValidateUsernameAsync(string username, int userId);
        void ValidatePassword(string password);
        void ValidateEmail(string email);
    }
}
