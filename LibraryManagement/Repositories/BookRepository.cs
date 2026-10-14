using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(b => b.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.Loans)
                .ThenInclude(l => l.Member)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ISBN == isbn.Trim());
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string? searchTerm, int? categoryId)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(b => b.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .AsQueryable();

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(b => 
                b.Title.ToLower().Contains(term) ||
                b.ISBN.ToLower().Contains(term) ||
                (b.Publisher != null && b.Publisher.ToLower().Contains(term)) ||
                b.BookAuthors.Any(ba => 
                    ba.Author.FirstName.ToLower().Contains(term) || 
                    ba.Author.LastName.ToLower().Contains(term)));
        }

        return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task UpdateBookAuthorsAsync(int bookId, IEnumerable<int> authorIds)
    {
        var existing = await _context.BookAuthors
            .Where(ba => ba.BookId == bookId)
            .ToListAsync();

        _context.BookAuthors.RemoveRange(existing);

        var newAuthorLinks = authorIds.Select(authorId => new BookAuthor
        {
            BookId = bookId,
            AuthorId = authorId
        });

        await _context.BookAuthors.AddRangeAsync(newAuthorLinks);
        await _context.SaveChangesAsync();
    }
}
