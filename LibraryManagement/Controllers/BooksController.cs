using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Books;
using LibraryManagement.Models.ViewModels.Common;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly ICategoryService _categoryService;
    private readonly IAuthorService _authorService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(
        IBookService bookService,
        ICategoryService categoryService,
        IAuthorService authorService,
        ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _categoryService = categoryService;
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, string? sortOrder, int page = 1)
    {
        const int pageSize = 8;
        page = Math.Max(1, page);

        var books = await _bookService.SearchBooksAsync(searchTerm, categoryId);

        // مرتب‌سازی بر اساس پارامتر انتخابی
        books = sortOrder switch
        {
            "title_asc" => books.OrderBy(b => b.Title),
            "title_desc" => books.OrderByDescending(b => b.Title),
            "year_asc" => books.OrderBy(b => b.PublishYear),
            "year_desc" => books.OrderByDescending(b => b.PublishYear),
            "copies_desc" => books.OrderByDescending(b => b.AvailableCopies),
            _ => books.OrderByDescending(b => b.CreatedAt)
        };

        var bookItems = books.Select(b => new BookItemViewModel
        {
            Id = b.Id,
            ISBN = b.ISBN,
            Title = b.Title,
            Publisher = b.Publisher,
            PublishYear = b.PublishYear,
            CategoryName = b.Category?.Name ?? "بدون دسته‌بندی",
            AuthorsDisplay = string.Join("، ", b.BookAuthors.Select(ba => ba.Author.FullName)),
            TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies
        }).ToList();

        var paginatedBooks = PaginatedList<BookItemViewModel>.Create(bookItems, page, pageSize);
        var categories = await _categoryService.GetAllCategoriesAsync();

        var viewModel = new BookListViewModel
        {
            Books = paginatedBooks,
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            SortOrder = sortOrder,
            CategoryList = new SelectList(categories, "Id", "Name", categoryId)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            TempData["ErrorMessage"] = "کتاب مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new BookDetailsViewModel
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Description = book.Description,
            Publisher = book.Publisher,
            PublishYear = book.PublishYear,
            PageCount = book.PageCount,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            CategoryName = book.Category?.Name ?? "بدون دسته‌بندی",
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            Authors = book.BookAuthors.Select(ba => ba.Author).ToList(),
            LoansHistory = book.Loans.OrderByDescending(l => l.LoanDate).Take(10).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new BookCreateViewModel();
        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var book = new Book
        {
            ISBN = model.ISBN.Trim(),
            Title = model.Title.Trim(),
            Description = model.Description,
            Publisher = model.Publisher,
            PublishYear = model.PublishYear,
            PageCount = model.PageCount,
            TotalCopies = model.TotalCopies,
            CategoryId = model.CategoryId
        };

        var result = await _bookService.CreateBookAsync(book, model.SelectedAuthorIds);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            TempData["ErrorMessage"] = "کتاب مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var model = new BookEditViewModel
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Description = book.Description,
            Publisher = book.Publisher,
            PublishYear = book.PublishYear,
            PageCount = book.PageCount,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            CategoryId = book.CategoryId,
            SelectedAuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToList()
        };

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var book = new Book
        {
            Id = model.Id,
            ISBN = model.ISBN.Trim(),
            Title = model.Title.Trim(),
            Description = model.Description,
            Publisher = model.Publisher,
            PublishYear = model.PublishYear,
            PageCount = model.PageCount,
            TotalCopies = model.TotalCopies,
            CategoryId = model.CategoryId
        };

        var result = await _bookService.UpdateBookAsync(book, model.SelectedAuthorIds);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateDropdownsAsync(model);
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
        var result = await _bookService.DeleteBookAsync(id);
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

    private async Task PopulateDropdownsAsync(BookCreateViewModel model)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var authors = await _authorService.GetAllAuthorsAsync();

        model.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
        model.AuthorList = new MultiSelectList(authors, "Id", "FullName", model.SelectedAuthorIds);
    }

    private async Task PopulateDropdownsAsync(BookEditViewModel model)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var authors = await _authorService.GetAllAuthorsAsync();

        model.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
        model.AuthorList = new MultiSelectList(authors, "Id", "FullName", model.SelectedAuthorIds);
    }
}
