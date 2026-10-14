using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class MemberRepository : BaseRepository<Member>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetByIdWithLoansAsync(int id)
    {
        return await _dbSet
            .Include(m => m.Loans)
                .ThenInclude(l => l.Book)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Member?> GetByNationalCodeAsync(string nationalCode)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.NationalCode == nationalCode.Trim());
    }

    public async Task<IEnumerable<Member>> SearchMembersAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await _dbSet.AsNoTracking().OrderBy(m => m.LastName).ToListAsync();
        }

        var term = query.Trim().ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(m => 
                m.FirstName.ToLower().Contains(term) ||
                m.LastName.ToLower().Contains(term) ||
                m.NationalCode.Contains(term) ||
                m.Phone.Contains(term))
            .OrderBy(m => m.LastName)
            .ToListAsync();
    }

    public async Task<bool> HasActiveLoansAsync(int memberId)
    {
        return await _context.Loans
            .AnyAsync(l => l.MemberId == memberId && (l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue));
    }
}
