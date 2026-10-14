using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class LoanRepository : BaseRepository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Loan>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<Loan?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(l => l.Book)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => (l.Status == LoanStatus.Overdue) || (l.Status == LoanStatus.Active && l.DueDate < now && l.ReturnDate == null))
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Book)
            .Where(l => l.MemberId == memberId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetRecentLoansAsync(int count = 5)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> CountActiveLoansAsync()
    {
        return await _dbSet.CountAsync(l => l.Status == LoanStatus.Active);
    }

    public async Task<int> CountOverdueLoansAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet.CountAsync(l => 
            l.Status == LoanStatus.Overdue || 
            (l.Status == LoanStatus.Active && l.DueDate < now && l.ReturnDate == null));
    }
}
