using LibraryManagement.Models.Entities;

namespace LibraryManagement.Services.Interfaces;

public record DashboardStats(
    int TotalBooks,
    int TotalCopies,
    int AvailableCopies,
    int TotalMembers,
    int ActiveLoans,
    int OverdueLoans,
    IEnumerable<Loan> RecentLoans
);

public interface IDashboardService
{
    Task<DashboardStats> GetDashboardStatsAsync();
}
