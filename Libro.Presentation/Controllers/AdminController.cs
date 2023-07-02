using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Libro.Presentation.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public AdminController(IUserManagementService userManagementService,IValidationService validationService, IMapper mapper)
        {
            _userManagementService = userManagementService;
            _validationService = validationService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userManagementService.AssignRoleAsync(model.Username, model.Role);

                return View(model);
            }
            return View(model);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpGet]
        public async Task<IActionResult> Patrons(string selectedPatron, int page = 1, int pageSize = 5)
        {
            var users = await _userManagementService.GetAllUsersAsync();
            var patronsDTOs = users.Where(p => p.Role.ToString() == "Patron");

            IEnumerable<UserDTO> filteredPatrons;
            if (!string.IsNullOrEmpty(selectedPatron))
            {
                filteredPatrons = patronsDTOs.Where(p => p.Username.Contains(selectedPatron));
            }
            else
            {
                filteredPatrons = patronsDTOs;
            }

            var pagedAllPatrons = patronsDTOs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pagedFilteredPatrons = filteredPatrons.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var patronsViewModel = new UsersViewModel
            {
                Users = pagedAllPatrons.ToList(),
                FilteredUsers = pagedFilteredPatrons.ToList(),
                SelectedUser = selectedPatron,

                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)filteredPatrons.Count() / pageSize)
            };

            return View(patronsViewModel);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpGet]
        public async Task<IActionResult> EditUser(int userId)
        {
            var user = await _userManagementService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = _mapper.Map<EditUserViewModel>(user);
            return View(userViewModel);
        }


        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel userDetailsModel)
        {
            try
            {
                var user = await _userManagementService.GetUserByIdAsync(userDetailsModel.UserId);

                var updatedUser = _mapper.Map<UserDTO>(userDetailsModel);
                updatedUser.Password = user.Password;
                updatedUser.Role = user.Role;

                // Skip username validation if it remains unchanged
                if (userDetailsModel.Username == user.Username)
                {
                    updatedUser.Username = user.Username;
                }
                else
                {
                    await _validationService.ValidateUsernameAsync(userDetailsModel.Username, userDetailsModel.UserId);
                }

                await _userManagementService.UpdateUserAsync(updatedUser.UserId, updatedUser);

                return RedirectToAction(updatedUser.Role.ToString()+"s");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(userDetailsModel);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Librarians(string selectedLibrarian, int page = 1, int pageSize = 5)
        {
            var users = await _userManagementService.GetAllUsersAsync();
            var librariansDTOs = users.Where(p => p.Role.ToString() == "Librarian");

            IEnumerable<UserDTO> filteredLibrarians;
            if (!string.IsNullOrEmpty(selectedLibrarian))
            {
                filteredLibrarians = librariansDTOs.Where(p => p.Username.Contains(selectedLibrarian));
            }
            else
            {
                filteredLibrarians = librariansDTOs;
            }

            var pagedAllLibrarians = librariansDTOs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var pagedFilteredLibrarians = filteredLibrarians.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var librariansViewModel = new UsersViewModel
            {
                Users = pagedAllLibrarians.ToList(),
                FilteredUsers = pagedFilteredLibrarians.ToList(),
                SelectedUser = selectedLibrarian,

                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)filteredLibrarians.Count() / pageSize)
            };

            return View(librariansViewModel);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userDTO = _mapper.Map<UserDTO>(model);
                    userDTO.Role = UserRole.Librarian;

                    await _userManagementService.CreateUserAsync(userDTO);

                    return RedirectToAction("Librarians");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
