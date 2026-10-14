using LibraryManagement.Models.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByIdWithBooksAsync(int id);
    Task<bool> HasBooksAsync(int categoryId);
}
