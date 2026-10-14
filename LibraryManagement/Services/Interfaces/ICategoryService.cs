using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Common;

namespace LibraryManagement.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<ServiceResult<Category>> CreateCategoryAsync(Category category);
    Task<ServiceResult> UpdateCategoryAsync(Category category);
    Task<ServiceResult> DeleteCategoryAsync(int id);
    Task<bool> IsNameUniqueAsync(string name, int? currentCategoryId = null);
}
