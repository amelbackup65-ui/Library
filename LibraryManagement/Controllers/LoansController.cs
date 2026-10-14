using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Common;
using LibraryManagement.Models.ViewModels.Loans;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IBookService _bookService;
    private readonly IMemberService _memberService;
    private readonly ILogger<LoansController> _logger;

    public LoansController(
        ILoanService loanService,
        IBookService bookService,
        IMemberService memberService,
        ILogger<LoansController> logger)
    {
        _loanService = loanService;
        _bookService = bookService;
        _memberService = memberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? statusFilter, string? searchTerm, int page = 1)
    {
        const int pageSize = 10;
        page = Math.Max(1, page);

        // بروزرسانی خودکار دیرکردها
        await _loanService.UpdateOverdueStatusAsync();

        var loans = await _loanService.GetAllLoansAsync();

        // فیلتر بر اساس وضعیت
        if (statusFilter == "active")
        {
            loans = loans.Where(l => l.Status == LoanStatus.Active);
        }
        else if (statusFilter == "returned")
        {
            loans = loans.Where(l => l.Status == LoanStatus.Returned);
        }
        else if (statusFilter == "overdue")
        {
            loans = loans.Where(l => l.Status == LoanStatus.Overdue || (l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow && l.ReturnDate == null));
        }

        // جستجو در عنوان کتاب، شابک، یا نام عضو
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            loans = loans.Where(l =>
                l.Book.Title.ToLower().Contains(term) ||
                l.Book.ISBN.ToLower().Contains(term) ||
                l.Member.FirstName.ToLower().Contains(term) ||
                l.Member.LastName.ToLower().Contains(term) ||
                l.Member.NationalCode.Contains(term));
        }

        var loanItems = loans.Select(l => new LoanItemViewModel
        {
            Id = l.Id,
            BookId = l.BookId,
            BookTitle = l.Book.Title,
            BookISBN = l.Book.ISBN,
            MemberId = l.MemberId,
            MemberName = l.Member.FullName,
            MemberPhone = l.Member.Phone,
            LoanDate = l.LoanDate,
            DueDate = l.DueDate,
            ReturnDate = l.ReturnDate,
            Status = l.Status,
            Notes = l.Notes
        }).OrderByDescending(l => l.LoanDate).ToList();

        var paginatedLoans = PaginatedList<LoanItemViewModel>.Create(loanItems, page, pageSize);

        var allLoans = await _loanService.GetAllLoansAsync();
        var viewModel = new LoanListViewModel
        {
            Loans = paginatedLoans,
            StatusFilter = statusFilter,
            SearchTerm = searchTerm,
            ActiveLoansCount = allLoans.Count(l => l.Status == LoanStatus.Active),
            OverdueLoansCount = allLoans.Count(l => l.Status == LoanStatus.Overdue || (l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow && l.ReturnDate == null))
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Overdue()
    {
        await _loanService.UpdateOverdueStatusAsync();
        var overdueLoans = await _loanService.GetOverdueLoansAsync();

        var now = DateTime.UtcNow;
        var viewModels = overdueLoans.Select(l => new OverdueLoanItemViewModel
        {
            Id = l.Id,
            BookTitle = l.Book.Title,
            BookISBN = l.Book.ISBN,
            MemberName = l.Member.FullName,
            MemberPhone = l.Member.Phone,
            LoanDate = l.LoanDate,
            DueDate = l.DueDate,
            DaysOverdue = Math.Max(1, (int)Math.Ceiling((now - l.DueDate).TotalDays))
        }).OrderByDescending(l => l.DaysOverdue).ToList();

        return View(viewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var loan = await _loanService.GetLoanByIdAsync(id);
        if (loan == null)
        {
            TempData["ErrorMessage"] = "رکورد امانت مورد نظر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new LoanDetailsViewModel
        {
            Id = loan.Id,
            Book = loan.Book,
            Member = loan.Member,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            Notes = loan.Notes
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? bookId = null, int? memberId = null)
    {
        var model = new LoanCreateViewModel
        {
            BookId = bookId ?? 0,
            MemberId = memberId ?? 0,
            LoanDays = 14
        };

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LoanCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        var result = await _loanService.IssueLoanAsync(model.BookId, model.MemberId, model.LoanDays, model.Notes);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id, string? returnUrl = null)
    {
        var result = await _loanService.ReturnLoanAsync(id);
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Extend(int id, int days = 7, string? returnUrl = null)
    {
        var result = await _loanService.ExtendLoanAsync(id, days);
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(LoanCreateViewModel model)
    {
        var allBooks = await _bookService.GetAllBooksAsync();
        // فقط کتاب‌هایی که نسخه آماده امانت دارند
        var availableBooks = allBooks
            .Where(b => b.AvailableCopies > 0)
            .Select(b => new
            {
                Id = b.Id,
                Title = $"{b.Title} (موجودی قفسه: {b.AvailableCopies} نسخه)"
            });

        var allMembers = await _memberService.GetAllMembersAsync();
        // فقط اعضایی که فعال هستند
        var activeMembers = allMembers
            .Where(m => m.IsActive)
            .Select(m => new
            {
                Id = m.Id,
                FullName = $"{m.FullName} (کد ملی: {m.NationalCode})"
            });

        model.AvailableBookList = new SelectList(availableBooks, "Id", "Title", model.BookId);
        model.ActiveMemberList = new SelectList(activeMembers, "Id", "FullName", model.MemberId);
    }
}
