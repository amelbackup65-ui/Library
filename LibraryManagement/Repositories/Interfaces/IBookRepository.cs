using LibraryManagement.Models.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetAllWithDetailsAsync();
    Task<Book?> GetByIdWithDetailsAsync(int id);
    Task<Book?> GetByISBNAsync(string isbn);
    Task<IEnumerable<Book>> SearchBooksAsync(string? searchTerm, int? categoryId);
    Task UpdateBookAuthorsAsync(int bookId, IEnumerable<int> authorIds);
}
