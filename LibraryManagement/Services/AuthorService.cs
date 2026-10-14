using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Services.Common;
using LibraryManagement.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IAuthorRepository authorRepository, ILogger<AuthorService> logger)
    {
        _authorRepository = authorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
    {
        return await _authorRepository.GetAllAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(int id)
    {
        return await _authorRepository.GetByIdWithBooksAsync(id);
    }

    public async Task<ServiceResult<Author>> CreateAuthorAsync(Author author)
    {
        if (string.IsNullOrWhiteSpace(author.FirstName) || string.IsNullOrWhiteSpace(author.LastName))
        {
            return ServiceResult<Author>.Fail("نام و نام خانوادگی نویسنده الزامی است.");
        }

        var created = await _authorRepository.AddAsync(author);
        _logger.LogInformation("نویسنده '{FullName}' (ID: {Id}) افزوده شد.", created.FullName, created.Id);
        return ServiceResult<Author>.Ok(created, "نویسنده با موفقیت ثبت شد.");
    }

    public async Task<ServiceResult> UpdateAuthorAsync(Author author)
    {
        var existing = await _authorRepository.GetByIdAsync(author.Id);
        if (existing == null)
        {
            return ServiceResult.Fail("نویسنده یافت نشد.");
        }

        existing.FirstName = author.FirstName.Trim();
        existing.LastName = author.LastName.Trim();
        existing.Biography = author.Biography;

        await _authorRepository.UpdateAsync(existing);
        _logger.LogInformation("اطلاعات نویسنده '{FullName}' به‌روزرسانی شد.", existing.FullName);
        return ServiceResult.Ok("اطلاعات نویسنده ویرایش شد.");
    }

    public async Task<ServiceResult> DeleteAuthorAsync(int id)
    {
        var author = await _authorRepository.GetByIdWithBooksAsync(id);
        if (author == null)
        {
            return ServiceResult.Fail("نویسنده یافت نشد.");
        }

        if (author.BookAuthors.Any())
        {
            return ServiceResult.Fail("به این نویسنده کتاب‌هایی متصل است و امکان حذف آن وجود ندارد.");
        }

        await _authorRepository.DeleteAsync(author);
        _logger.LogInformation("نویسنده با شناسه {Id} حذف شد.", id);
        return ServiceResult.Ok("نویسنده با موفقیت حذف شد.");
    }
}
