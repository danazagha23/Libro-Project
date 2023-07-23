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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Libro.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IReadingListService _readingListRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Application.ServicesInterfaces.IAuthenticationService _authenticationService;

        public AccountController(
            IUserManagementService userManagementService,
            IReadingListService readingListService,
            IMapper mapper,
            Application.ServicesInterfaces.IAuthenticationService authenticationService,
            IConfiguration configuration)
        {
            _userManagementService = userManagementService;
            _readingListRepository = readingListService;
            _mapper = mapper;
            _authenticationService = authenticationService;
            _configuration = configuration;
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
                    var userDTO = _mapper.Map<UserDTO>(model);
                    userDTO.Role = UserRole.Patron;

                    await _userManagementService.CreateUserAsync(userDTO);

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

                    var secretKey = _configuration["JwtSettings:SecretKey"];
                    var issuer = _configuration["JwtSettings:Issuer"];
                    var audience = _configuration["JwtSettings:Audience"];

                    // Generate JWT token
                    var token = GenerateJwtToken(userDTO.UserId.ToString(), userDTO.Username, userDTO.Role.ToString(),
                        secretKey, issuer, audience);

                    // Create a cookie with the token
                    Response.Cookies.Append("accessToken", token, new CookieOptions
                    {
                        HttpOnly = true, // HttpOnly cookie for security
                        Expires = DateTime.UtcNow.AddHours(1) // Set cookie expiration time
                    });

                    // Return the JWT token as a response
                    return RedirectToAction("Index", "Home"); ;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        public string GenerateJwtToken(string userId, string username, string role, string secretKey, string issuer, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: credentials
            );

            return tokenHandler.WriteToken(token);
        }

        [Authorize(Roles = "Patron")]
        [HttpGet]
        public async Task<IActionResult> ProfileAsync()
        {
            // Get the user's unique identifier from the claims
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userDTO = await _userManagementService.GetUserByIdAsync(userId);

            var borrowingHistory = await _userManagementService.GetBorrowingHistoryAsync(userId);
            var currentLoans = await _userManagementService.GetCurrentLoansAsync(userId);
            var overdueLoans = await _userManagementService.GetOverdueLoansAsync(userId);
            var readingLists = await _readingListRepository.GetReadingListsByUserIdAsync(userId); // Retrieve the reading lists

            var userProfile = new UserProfileViewModel
            {
                User = userDTO,
                BorrowingHistory = borrowingHistory.ToList(),
                CurrentLoans = currentLoans.ToList(),
                OverdueLoans = overdueLoans.ToList(),
                ReadingLists = readingLists.ToList() 
            };

            return View(userProfile);
        }

        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Append("accessToken", "", new CookieOptions
            {
                HttpOnly = true,
                // Set the same Site and Path as you used when creating the cookie
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1) // Set the expiration to a past date
            }); 

            return RedirectToAction("Index", "Home");
        }
    }
}
