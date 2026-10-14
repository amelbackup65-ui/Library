using LibraryManagement.Models.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface ILoanRepository : IRepository<Loan>
{
    Task<IEnumerable<Loan>> GetAllWithDetailsAsync();
    Task<Loan?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId);
    Task<IEnumerable<Loan>> GetRecentLoansAsync(int count = 5);
    Task<int> CountActiveLoansAsync();
    Task<int> CountOverdueLoansAsync();
}
