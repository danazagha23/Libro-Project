using AutoMapper;
using Libro.Application.ServicesInterfaces;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Libro.Presentation.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IUserManagementService _userManagementService;

        public AdminController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            // Validate the role assignment data
            if (ModelState.IsValid)
            {
                // Assign the role to the specified user
                await _userManagementService.AssignRoleAsync(model.Username, model.Role);

                return View(model);
            }
            return View(model);
        }
    }
}
