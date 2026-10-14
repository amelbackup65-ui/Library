using LibraryManagement.Models.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IMemberRepository : IRepository<Member>
{
    Task<Member?> GetByIdWithLoansAsync(int id);
    Task<Member?> GetByNationalCodeAsync(string nationalCode);
    Task<IEnumerable<Member>> SearchMembersAsync(string query);
    Task<bool> HasActiveLoansAsync(int memberId);
}
