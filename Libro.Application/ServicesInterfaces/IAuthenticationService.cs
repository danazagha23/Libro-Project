using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ServicesInterfaces
{
    public interface IAuthenticationService
    {
        Task SignInAsync(HttpContext context, string authenticationScheme, ClaimsPrincipal principal);
    }
}
