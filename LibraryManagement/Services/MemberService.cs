using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Services.Common;
using LibraryManagement.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<MemberService> _logger;

    public MemberService(IMemberRepository memberRepository, ILogger<MemberService> logger)
    {
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return await _memberRepository.GetAllAsync();
    }

    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        return await _memberRepository.GetByIdWithLoansAsync(id);
    }

    public async Task<IEnumerable<Member>> SearchMembersAsync(string query)
    {
        return await _memberRepository.SearchMembersAsync(query);
    }

    public async Task<ServiceResult<Member>> CreateMemberAsync(Member member)
    {
        if (!await IsNationalCodeUniqueAsync(member.NationalCode))
        {
            return ServiceResult<Member>.Fail($"کد ملی '{member.NationalCode}' قبلاً در سیستم ثبت شده است.");
        }

        member.RegisterDate = DateTime.UtcNow;
        member.IsActive = true;

        var created = await _memberRepository.AddAsync(member);
        _logger.LogInformation("عضو جدید با نام '{FullName}' و شناسه {Id} ثبت‌نام شد.", created.FullName, created.Id);

        return ServiceResult<Member>.Ok(created, "عضو جدید با موفقیت ثبت شد.");
    }

    public async Task<ServiceResult> UpdateMemberAsync(Member member)
    {
        var existing = await _memberRepository.GetByIdAsync(member.Id);
        if (existing == null)
        {
            return ServiceResult.Fail("عضو مورد نظر یافت نشد.");
        }

        if (!await IsNationalCodeUniqueAsync(member.NationalCode, member.Id))
        {
            return ServiceResult.Fail($"کد ملی '{member.NationalCode}' متعلق به عضو دیگری است.");
        }

        existing.FirstName = member.FirstName.Trim();
        existing.LastName = member.LastName.Trim();
        existing.NationalCode = member.NationalCode.Trim();
        existing.Phone = member.Phone.Trim();
        existing.Email = member.Email?.Trim();
        existing.Address = member.Address?.Trim();
        existing.IsActive = member.IsActive;

        await _memberRepository.UpdateAsync(existing);
        _logger.LogInformation("مشخصات عضو '{FullName}' (ID: {Id}) به‌روزرسانی شد.", existing.FullName, existing.Id);

        return ServiceResult.Ok("اطلاعات عضو با موفقیت ویرایش گردید.");
    }

    public async Task<ServiceResult> DeleteMemberAsync(int id)
    {
        var member = await _memberRepository.GetByIdWithLoansAsync(id);
        if (member == null)
        {
            return ServiceResult.Fail("عضو یافت نشد.");
        }

        if (member.Loans.Any(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
        {
            return ServiceResult.Fail("این عضو دارای امانت‌های فعال یا دیرکرد تسویه‌نشده است و امکان حذف آن وجود ندارد.");
        }

        await _memberRepository.DeleteAsync(member);
        _logger.LogInformation("عضو با شناسه {Id} حذف شد.", id);
        return ServiceResult.Ok("عضو با موفقیت از سیستم حذف شد.");
    }

    public async Task<ServiceResult> ToggleMemberStatusAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
        {
            return ServiceResult.Fail("عضو یافت نشد.");
        }

        member.IsActive = !member.IsActive;
        await _memberRepository.UpdateAsync(member);

        string statusText = member.IsActive ? "فعال" : "غیرفعال";
        _logger.LogInformation("وضعیت عضو {MemberId} به {Status} تغییر یافت.", id, statusText);

        return ServiceResult.Ok($"وضعیت عضو به {statusText} تغییر یافت.");
    }

    public async Task<bool> IsNationalCodeUniqueAsync(string nationalCode, int? currentMemberId = null)
    {
        var trimmed = nationalCode.Trim();
        if (currentMemberId.HasValue)
        {
            return !await _memberRepository.ExistsAsync(m => m.NationalCode == trimmed && m.Id != currentMemberId.Value);
        }
        return !await _memberRepository.ExistsAsync(m => m.NationalCode == trimmed);
    }
}
