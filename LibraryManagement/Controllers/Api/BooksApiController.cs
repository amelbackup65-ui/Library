using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers.Api;

[ApiController]
[Route("api/books")]
[Produces("application/json")]
public class BooksApiController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksApiController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// جستجو و دریافت لیست کتاب‌ها به همراه اطلاعات موجودی
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _bookService.GetBooksPagedAsync(page, pageSize, search);
        return Ok(new
        {
            total = result.TotalCount,
            page = result.PageIndex,
            pageSize = result.PageSize,
            totalPages = result.TotalPages,
            items = result.Items.Select(b => new
            {
                id = b.Id,
                title = b.Title,
                isbn = b.ISBN,
                category = b.CategoryName,
                totalCopies = b.TotalCopies,
                availableCopies = b.AvailableCopies,
                isAvailable = b.AvailableCopies > 0
            })
        });
    }

    /// <summary>
    /// استعلام وضعیت و موجودی یک کتاب با شابک (ISBN)
    /// </summary>
    [HttpGet("by-isbn/{isbn}")]
    public async Task<IActionResult> GetByIsbn(string isbn)
    {
        var book = await _bookService.GetBookByIsbnAsync(isbn);
        if (book == null)
        {
            return NotFound(new { message = $"کتابی با شابک {isbn} یافت نشد." });
        }

        return Ok(new
        {
            id = book.Id,
            title = book.Title,
            isbn = book.ISBN,
            category = book.Category?.Name,
            availableCopies = book.AvailableCopies,
            isAvailable = book.AvailableCopies > 0
        });
    }
}
