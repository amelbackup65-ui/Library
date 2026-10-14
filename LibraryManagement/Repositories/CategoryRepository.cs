using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByIdWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> HasBooksAsync(int categoryId)
    {
        return await _context.Books.AnyAsync(b => b.CategoryId == categoryId);
    }
}
