using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IEmailService
    {
        Task SendEmail(string recipientEmail, string subject, string message);
    }
}
