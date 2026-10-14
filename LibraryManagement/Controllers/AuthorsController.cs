using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Authors;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class AuthorsController : Controller
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        var viewModels = authors.Select(a => new AuthorItemViewModel
        {
            Id = a.Id,
            FullName = a.FullName,
            Biography = a.Biography,
            BooksCount = a.BookAuthors.Count
        }).OrderBy(a => a.FullName).ToList();

        return View(viewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            TempData["ErrorMessage"] = "نویسنده مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new AuthorDetailsViewModel
        {
            Id = author.Id,
            FullName = author.FullName,
            Biography = author.Biography,
            Books = author.BookAuthors.Select(ba => new AuthorBookItemViewModel
            {
                Id = ba.Book.Id,
                ISBN = ba.Book.ISBN,
                Title = ba.Book.Title,
                PublishYear = ba.Book.PublishYear,
                AvailableCopies = ba.Book.AvailableCopies
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AuthorFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var author = new Author
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Biography = model.Biography
        };

        var result = await _authorService.CreateAuthorAsync(author);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var author = await _authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            TempData["ErrorMessage"] = "نویسنده مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var model = new AuthorFormViewModel
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            Biography = author.Biography
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AuthorFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var author = new Author
        {
            Id = model.Id.Value,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Biography = model.Biography
        };

        var result = await _authorService.UpdateAuthorAsync(author);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _authorService.DeleteAuthorAsync(id);
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
