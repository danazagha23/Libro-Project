using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Extensions;
using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Helpers;
using Libro.Presentation.Models;
using MailKit.Search;
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
        private readonly IPaginationWrapper<UserDTO> _paginationWrapper;

        public AdminController(IUserManagementService userManagementService,IValidationService validationService, IMapper mapper, IPaginationWrapper<UserDTO> paginationWrapper)
        {
            _userManagementService = userManagementService;
            _validationService = validationService;
            _mapper = mapper;
            _paginationWrapper = paginationWrapper;
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
            var patronsDTOs = await _userManagementService.GetUsersByRoleAsync(UserRole.Patron);

            ICollection<UserDTO> filteredPatrons;
            if (!string.IsNullOrEmpty(selectedPatron))
            {
                filteredPatrons = patronsDTOs.Where(p => p.Username.ContainsIgnoreCaseAndWhitespace(selectedPatron)).ToList();
            }
            else
            {
                filteredPatrons = patronsDTOs.ToList();
            }

            var pagedFilteredPatrons = _paginationWrapper.GetPage(filteredPatrons, page, pageSize).ToList();

            var patronsViewModel = new UsersViewModel
            {
                Users = patronsDTOs.ToList(),
                FilteredUsers = pagedFilteredPatrons.ToList(),
                SelectedUser = selectedPatron,

                PageNumber = page,
                TotalPages = _paginationWrapper.GetTotalPages(filteredPatrons.ToList(), pageSize)
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
            var librarians = await _userManagementService.GetUsersByRoleAsync(UserRole.Librarian);

            ICollection<UserDTO> filteredLibrarians;
            if (!string.IsNullOrEmpty(selectedLibrarian))
            {
                filteredLibrarians = librarians.Where(p => p.Username.ContainsIgnoreCaseAndWhitespace(selectedLibrarian)).ToList();
            }
            else
            {
                filteredLibrarians = librarians.ToList();
            }

            var pagedFilteredLibrarians = _paginationWrapper.GetPage(filteredLibrarians, page, pageSize).ToList();

            var librariansViewModel = new UsersViewModel
            {
                Users = librarians.ToList(),
                FilteredUsers = pagedFilteredLibrarians.ToList(),
                SelectedUser = selectedLibrarian,

                PageNumber = page,
                TotalPages = _paginationWrapper.GetTotalPages(filteredLibrarians.ToList(), pageSize)
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
