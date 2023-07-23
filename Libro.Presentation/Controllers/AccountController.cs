using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IMapper _mapper;

        public AccountController(IUserManagementService userManagementService, IMapper mapper)
        {
            _userManagementService = userManagementService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Map the RegisterViewModel to UserDTO 
                    var userDTO = _mapper.Map<UserDTO>(model);
                    userDTO.Role = UserRole.Patron;

                    // Register the user
                    await _userManagementService.CreateUserAsync(userDTO);

                    // Optionally, you can redirect to a login page or another action
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Authenticate the user and get the user DTO
                    var userDTO = await _userManagementService.AuthenticateUserAsync(model.Username, model.Password);

                    if (userDTO == null)
                    {
                        ModelState.AddModelError("", "Invalid username or password");
                        return View(model);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userDTO.UserId.ToString()),
                        new Claim(ClaimTypes.Name, userDTO.Username),
                        new Claim(ClaimTypes.Role, userDTO.Role.ToString())
                    };

                    var identity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal);

                    return RedirectToAction("Profile", "Account");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProfileAsync()
        {
            // Get the user's unique identifier from the claims
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userDTO = await _userManagementService.GetUserByIdAsync(userId);
            var userProfile = _mapper.Map<UserProfileViewModel>(userDTO);

            return View(userProfile);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

    }
}
