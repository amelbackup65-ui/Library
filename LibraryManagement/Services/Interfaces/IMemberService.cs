using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Common;

namespace LibraryManagement.Services.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
    Task<Member?> GetMemberByIdAsync(int id);
    Task<IEnumerable<Member>> SearchMembersAsync(string query);
    Task<ServiceResult<Member>> CreateMemberAsync(Member member);
    Task<ServiceResult> UpdateMemberAsync(Member member);
    Task<ServiceResult> DeleteMemberAsync(int id);
    Task<ServiceResult> ToggleMemberStatusAsync(int id);
    Task<bool> IsNationalCodeUniqueAsync(string nationalCode, int? currentMemberId = null);
}
