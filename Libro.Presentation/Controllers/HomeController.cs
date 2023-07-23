using Libro.Application.ServicesInterfaces;
using Libro.Domain.Enums;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Libro.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookManagementService _bookManagementService;

        public HomeController(ILogger<HomeController> logger, IBookManagementService bookManagementService)
        {
            _logger = logger;
            _bookManagementService = bookManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var books = await _bookManagementService.GetAllBooksAsync();
            var availableBooks = books.Where(b => b.AvailabilityStatus == AvailabilityStatus.Available);
            var homeViewModel = new HomeViewModel
            {
                AvailableBooks = availableBooks
            };

            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}