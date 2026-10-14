using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Services.Common;
using LibraryManagement.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdWithBooksAsync(id);
    }

    public async Task<ServiceResult<Category>> CreateCategoryAsync(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            return ServiceResult<Category>.Fail("نام دسته‌بندی نمی‌تواند خالی باشد.");
        }

        if (!await IsNameUniqueAsync(category.Name))
        {
            return ServiceResult<Category>.Fail($"دسته‌بندی با نام '{category.Name}' قبلاً وجود دارد.");
        }

        var created = await _categoryRepository.AddAsync(category);
        _logger.LogInformation("دسته‌بندی '{Name}' (ID: {Id}) ایجاد شد.", created.Name, created.Id);
        return ServiceResult<Category>.Ok(created, "دسته‌بندی جدید با موفقیت ایجاد شد.");
    }

    public async Task<ServiceResult> UpdateCategoryAsync(Category category)
    {
        var existing = await _categoryRepository.GetByIdAsync(category.Id);
        if (existing == null)
        {
            return ServiceResult.Fail("دسته‌بندی مورد نظر یافت نشد.");
        }

        if (!await IsNameUniqueAsync(category.Name, category.Id))
        {
            return ServiceResult.Fail($"دسته‌بندی دیگری با نام '{category.Name}' وجود دارد.");
        }

        existing.Name = category.Name.Trim();
        existing.Description = category.Description;

        await _categoryRepository.UpdateAsync(existing);
        _logger.LogInformation("دسته‌بندی '{Name}' (ID: {Id}) به‌روزرسانی شد.", existing.Name, existing.Id);
        return ServiceResult.Ok("دسته‌بندی با موفقیت ویرایش شد.");
    }

    public async Task<ServiceResult> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdWithBooksAsync(id);
        if (category == null)
        {
            return ServiceResult.Fail("دسته‌بندی یافت نشد.");
        }

        if (category.Books.Any())
        {
            return ServiceResult.Fail("کتاب‌هایی به این دسته‌بندی اختصاص دارند؛ ابتدا کتاب‌ها را به دسته‌بندی دیگری منتقل کنید.");
        }

        await _categoryRepository.DeleteAsync(category);
        _logger.LogInformation("دسته‌بندی با شناسه {Id} حذف شد.", id);
        return ServiceResult.Ok("دسته‌بندی با موفقیت حذف گردید.");
    }

    public async Task<bool> IsNameUniqueAsync(string name, int? currentCategoryId = null)
    {
        var trimmed = name.Trim();
        if (currentCategoryId.HasValue)
        {
            return !await _categoryRepository.ExistsAsync(c => c.Name == trimmed && c.Id != currentCategoryId.Value);
        }
        return !await _categoryRepository.ExistsAsync(c => c.Name == trimmed);
    }
}
