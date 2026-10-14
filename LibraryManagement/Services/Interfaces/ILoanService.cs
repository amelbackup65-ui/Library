using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Common;

namespace LibraryManagement.Services.Interfaces;

public interface ILoanService
{
    Task<IEnumerable<Loan>> GetAllLoansAsync();
    Task<Loan?> GetLoanByIdAsync(int id);
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId);
    Task<ServiceResult<Loan>> IssueLoanAsync(int bookId, int memberId, int loanDays, string? notes = null);
    Task<ServiceResult> ReturnLoanAsync(int loanId);
    Task<ServiceResult> ExtendLoanAsync(int loanId, int additionalDays = 7);
    Task<int> UpdateOverdueStatusAsync();
}
