using AutoMapper;
using Libro.Application.DTOs;
using Libro.Application.Services;
using Libro.Application.ServicesInterfaces;
using Libro.Presentation.Helpers;
using Libro.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libro.Presentation.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorManagementService _authorManagementService;
        private readonly IMapper _mapper;
        private readonly IPaginationWrapper<AuthorDTO> _paginationWrapper;

        public AuthorsController(IAuthorManagementService authorManagementService, IMapper mapper, IPaginationWrapper<AuthorDTO> paginationWrapper)
        {
            _authorManagementService = authorManagementService;
            _mapper = mapper;   
            _paginationWrapper = paginationWrapper;
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> Index(string selectedAuthor, int page = 1, int pageSize = 5)
        {
            var authors = await _authorManagementService.GetAllAuthorsAsync();

            ICollection<AuthorDTO> filteredAuthors;
            if (!string.IsNullOrEmpty(selectedAuthor))
            {
                filteredAuthors = authors.Where(a => a.AuthorName.Contains(selectedAuthor)).ToList();
            }
            else
            {
                filteredAuthors = authors;
            }

            var pagedFilteredAuthors = _paginationWrapper.GetPage(filteredAuthors, page, pageSize).ToList();

            var authorsViewModel = new AuthorsViewModel
            {
                Authors = authors.ToList(),
                FilteredAuthors = pagedFilteredAuthors.ToList(),
                SelectedAuthor = selectedAuthor,

                PageNumber = page,
                TotalPages = _paginationWrapper.GetTotalPages(filteredAuthors.ToList(), pageSize)
            };

            return View(authorsViewModel);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> EditAuthor(int authorId)
        {
            var author = await _authorManagementService.GetAuthorByIdAsync(authorId);

            if (author == null)
            {
                return NotFound();
            }

            var authorViewModel = _mapper.Map<AuthorViewModel>(author);
            return View(authorViewModel);
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> EditAuthor(AuthorViewModel authorDetailsModel)
        {
            try
            {
                var author = await _authorManagementService.GetAuthorByIdAsync(authorDetailsModel.AuthorId);

                var updatedAuthor = _mapper.Map<AuthorDTO>(authorDetailsModel);

                await _authorManagementService.UpdateAuthorAsync(updatedAuthor.AuthorId, updatedAuthor);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(authorDetailsModel);
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            try
            {
                await _authorManagementService.DeleteAuthorAsync(authorId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<IActionResult> CreateAuthor()
        {
            return View();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(AuthorViewModel authorViewModel)
        {
            var author = _mapper.Map<AuthorDTO>(authorViewModel);

            await _authorManagementService.CreateAuthorAsync(author);

            return Redirect("Index");
        }
    }
}
