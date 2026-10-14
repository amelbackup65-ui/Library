using System.Linq.Expressions;

namespace LibraryManagement.Repositories.Interfaces;

/// <summary>
/// اینترفیس جنریک ریپازیتوری پایه جهت عملیات استاندارد داده‌ای
/// </summary>
/// <typeparam name="T">موجودیت دامنه</typeparam>
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<int> SaveChangesAsync();
}
