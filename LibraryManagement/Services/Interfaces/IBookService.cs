using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Common;

namespace LibraryManagement.Services.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<IEnumerable<Book>> SearchBooksAsync(string? searchTerm, int? categoryId);
    Task<ServiceResult<Book>> CreateBookAsync(Book book, IEnumerable<int> authorIds);
    Task<ServiceResult> UpdateBookAsync(Book book, IEnumerable<int> authorIds);
    Task<ServiceResult> DeleteBookAsync(int id);
    Task<bool> IsIsbnUniqueAsync(string isbn, int? currentBookId = null);
}
