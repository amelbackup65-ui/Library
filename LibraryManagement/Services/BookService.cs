using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Services.Common;
using LibraryManagement.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<BookService> _logger;

    public BookService(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IAuthorRepository authorRepository,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
        _authorRepository = authorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllWithDetailsAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string? searchTerm, int? categoryId)
    {
        return await _bookRepository.SearchBooksAsync(searchTerm, categoryId);
    }

    public async Task<ServiceResult<Book>> CreateBookAsync(Book book, IEnumerable<int> authorIds)
    {
        // ۱. اعتبارسنجی شابک یکتا
        if (!await IsIsbnUniqueAsync(book.ISBN))
        {
            return ServiceResult<Book>.Fail($"کتابی با شابک '{book.ISBN}' قبلاً در سیستم ثبت شده است.");
        }

        // ۲. اعتبارسنجی تعداد نسخه‌ها
        if (book.TotalCopies < 1)
        {
            return ServiceResult<Book>.Fail("تعداد کل نسخه‌های کتاب باید حداقل ۱ باشد.");
        }

        book.AvailableCopies = book.TotalCopies;
        book.CreatedAt = DateTime.UtcNow;

        // ۳. بررسی دسته‌بندی
        var categoryExists = await _categoryRepository.ExistsAsync(c => c.Id == book.CategoryId);
        if (!categoryExists)
        {
            return ServiceResult<Book>.Fail("دسته‌بندی انتخاب شده معتبر نیست.");
        }

        // ۴. ایجاد کتاب
        var created = await _bookRepository.AddAsync(book);

        // ۵. انتساب نویسندگان
        var authorIdList = authorIds?.ToList() ?? new List<int>();
        if (authorIdList.Any())
        {
            await _bookRepository.UpdateBookAuthorsAsync(created.Id, authorIdList);
        }

        _logger.LogInformation("کتاب جدید با عنوان '{Title}' و شناسه {Id} ثبت شد.", created.Title, created.Id);
        return ServiceResult<Book>.Ok(created, "کتاب با موفقیت ایجاد شد.");
    }

    public async Task<ServiceResult> UpdateBookAsync(Book book, IEnumerable<int> authorIds)
    {
        var existing = await _bookRepository.GetByIdWithDetailsAsync(book.Id);
        if (existing == null)
        {
            return ServiceResult.Fail("کتاب مورد نظر یافت نشد.");
        }

        // اعتبارسنجی شابک یکتا در ویرایش
        if (!await IsIsbnUniqueAsync(book.ISBN, book.Id))
        {
            return ServiceResult.Fail($"شابک '{book.ISBN}' متعلق به کتاب دیگری در سیستم است.");
        }

        // تنظیم موجودی در دسترس در صورت تغییر تعداد کل نسخه‌ها
        int copiesDifference = book.TotalCopies - existing.TotalCopies;
        int newAvailable = existing.AvailableCopies + copiesDifference;

        if (newAvailable < 0)
        {
            return ServiceResult.Fail("کاهش تعداد کل نسخه‌ها کمتر از تعداد نسخه‌هایی است که هم‌اکنون در امانت هستند.");
        }

        existing.ISBN = book.ISBN.Trim();
        existing.Title = book.Title.Trim();
        existing.Description = book.Description;
        existing.Publisher = book.Publisher;
        existing.PublishYear = book.PublishYear;
        existing.PageCount = book.PageCount;
        existing.CategoryId = book.CategoryId;
        existing.TotalCopies = book.TotalCopies;
        existing.AvailableCopies = newAvailable;
        existing.UpdatedAt = DateTime.UtcNow;

        await _bookRepository.UpdateAsync(existing);

        // به‌روزرسانی نویسندگان
        var authorIdList = authorIds?.ToList() ?? new List<int>();
        await _bookRepository.UpdateBookAuthorsAsync(existing.Id, authorIdList);

        _logger.LogInformation("اطلاعات کتاب '{Title}' (ID: {Id}) به‌روزرسانی شد.", existing.Title, existing.Id);
        return ServiceResult.Ok("اطلاعات کتاب با موفقیت ویرایش شد.");
    }

    public async Task<ServiceResult> DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdWithDetailsAsync(id);
        if (book == null)
        {
            return ServiceResult.Fail("کتاب یافت نشد.");
        }

        if (book.Loans.Any(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
        {
            return ServiceResult.Fail("این کتاب در حال حاضر دارای امانت فعال است و امکان حذف آن وجود ندارد.");
        }

        await _bookRepository.DeleteAsync(book);
        _logger.LogInformation("کتاب '{Title}' (ID: {Id}) حذف شد.", book.Title, id);
        return ServiceResult.Ok("کتاب با موفقیت حذف گردید.");
    }

    public async Task<bool> IsIsbnUniqueAsync(string isbn, int? currentBookId = null)
    {
        var trimmed = isbn.Trim();
        if (currentBookId.HasValue)
        {
            return !await _bookRepository.ExistsAsync(b => b.ISBN == trimmed && b.Id != currentBookId.Value);
        }
        return !await _bookRepository.ExistsAsync(b => b.ISBN == trimmed);
    }
}
