using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Common;

namespace LibraryManagement.Services.Interfaces;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(int id);
    Task<ServiceResult<Author>> CreateAuthorAsync(Author author);
    Task<ServiceResult> UpdateAuthorAsync(Author author);
    Task<ServiceResult> DeleteAuthorAsync(int id);
}
