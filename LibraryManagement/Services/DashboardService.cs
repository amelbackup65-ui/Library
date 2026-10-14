using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly ILoanRepository _loanRepository;

    public DashboardService(ApplicationDbContext context, ILoanRepository loanRepository)
    {
        _context = context;
        _loanRepository = loanRepository;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var totalBooks = await _context.Books.CountAsync();
        var totalCopies = await _context.Books.SumAsync(b => (int?)b.TotalCopies) ?? 0;
        var availableCopies = await _context.Books.SumAsync(b => (int?)b.AvailableCopies) ?? 0;
        var totalMembers = await _context.Members.CountAsync();

        var activeLoans = await _loanRepository.CountActiveLoansAsync();
        var overdueLoans = await _loanRepository.CountOverdueLoansAsync();
        var recentLoans = await _loanRepository.GetRecentLoansAsync(6);

        return new DashboardStats(
            TotalBooks: totalBooks,
            TotalCopies: totalCopies,
            AvailableCopies: availableCopies,
            TotalMembers: totalMembers,
            ActiveLoans: activeLoans,
            OverdueLoans: overdueLoans,
            RecentLoans: recentLoans
        );
    }
}
