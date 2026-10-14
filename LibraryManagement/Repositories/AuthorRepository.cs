using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
{
    public AuthorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Author?> GetByIdWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Author>> GetAuthorsByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.ToList();
        return await _dbSet
            .Where(a => idList.Contains(a.Id))
            .ToListAsync();
    }
}
