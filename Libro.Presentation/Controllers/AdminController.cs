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

            var patronsViewModel = new PatronsViewModel
            {
                Patrons = pagedAllPatrons.ToList(),
                FilteredPatrons = pagedFilteredPatrons.ToList(),
                SelectedPatron = selectedPatron,

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

                return RedirectToAction("Patrons");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(userDetailsModel);
            }
    }
    }
}
