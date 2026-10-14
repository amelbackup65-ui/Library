using LibraryManagement.Models.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByIdWithBooksAsync(int id);
    Task<IEnumerable<Author>> GetAuthorsByIdsAsync(IEnumerable<int> ids);
}
