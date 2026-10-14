using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Common;
using LibraryManagement.Models.ViewModels.Members;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberService memberService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, string? statusFilter, int page = 1)
    {
        const int pageSize = 10;
        page = Math.Max(1, page);

        var members = string.IsNullOrWhiteSpace(searchTerm)
            ? await _memberService.GetAllMembersAsync()
            : await _memberService.SearchMembersAsync(searchTerm);

        // اعمال فیلتر وضعیت فعال/غیرفعال
        if (statusFilter == "active")
        {
            members = members.Where(m => m.IsActive);
        }
        else if (statusFilter == "inactive")
        {
            members = members.Where(m => !m.IsActive);
        }

        var memberItems = members.Select(m => new MemberItemViewModel
        {
            Id = m.Id,
            FullName = m.FullName,
            NationalCode = m.NationalCode,
            Phone = m.Phone,
            Email = m.Email,
            RegisterDate = m.RegisterDate,
            IsActive = m.IsActive,
            ActiveLoansCount = m.Loans.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue)
        }).OrderByDescending(m => m.RegisterDate).ToList();

        var paginatedMembers = PaginatedList<MemberItemViewModel>.Create(memberItems, page, pageSize);

        var viewModel = new MemberListViewModel
        {
            Members = paginatedMembers,
            SearchTerm = searchTerm,
            StatusFilter = statusFilter
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            TempData["ErrorMessage"] = "عضو مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new MemberDetailsViewModel
        {
            Id = member.Id,
            FullName = member.FullName,
            NationalCode = member.NationalCode,
            Phone = member.Phone,
            Email = member.Email,
            Address = member.Address,
            RegisterDate = member.RegisterDate,
            IsActive = member.IsActive,
            ActiveLoansCount = member.Loans.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue),
            LoansHistory = member.Loans.OrderByDescending(l => l.LoanDate).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new MemberFormViewModel { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MemberFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var member = new Member
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            NationalCode = model.NationalCode.Trim(),
            Phone = model.Phone.Trim(),
            Email = model.Email?.Trim(),
            Address = model.Address?.Trim(),
            IsActive = model.IsActive
        };

        var result = await _memberService.CreateMemberAsync(member);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            TempData["ErrorMessage"] = "عضو مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var model = new MemberFormViewModel
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            NationalCode = member.NationalCode,
            Phone = member.Phone,
            Email = member.Email,
            Address = member.Address,
            IsActive = member.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MemberFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var member = new Member
        {
            Id = model.Id.Value,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            NationalCode = model.NationalCode.Trim(),
            Phone = model.Phone.Trim(),
            Email = model.Email?.Trim(),
            Address = model.Address?.Trim(),
            IsActive = model.IsActive
        };

        var result = await _memberService.UpdateMemberAsync(member);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _memberService.ToggleMemberStatusAsync(id);
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _memberService.DeleteMemberAsync(id);
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
