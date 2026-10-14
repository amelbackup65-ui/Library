using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Services.Common;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LoanService> _logger;

    public LoanService(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        ApplicationDbContext context,
        ILogger<LoanService> logger)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Loan>> GetAllLoansAsync()
    {
        return await _loanRepository.GetAllWithDetailsAsync();
    }

    public async Task<Loan?> GetLoanByIdAsync(int id)
    {
        return await _loanRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        return await _loanRepository.GetActiveLoansAsync();
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        return await _loanRepository.GetOverdueLoansAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId)
    {
        return await _loanRepository.GetLoansByMemberIdAsync(memberId);
    }

    /// <summary>
    /// ثبت امانت با تضمین سازگاری داده و تراکنش اتمیک (ACID Transaction)
    /// </summary>
    public async Task<ServiceResult<Loan>> IssueLoanAsync(int bookId, int memberId, int loanDays, string? notes = null)
    {
        if (loanDays <= 0)
        {
            return ServiceResult<Loan>.Fail("مدت زمان امانت باید حداقل ۱ روز باشد.");
        }

        // ۱. بررسی وضعیت عضو
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
        {
            return ServiceResult<Loan>.Fail("عضو مورد نظر یافت نشد.");
        }

        if (!member.IsActive)
        {
            return ServiceResult<Loan>.Fail("امکان امانت به عضو غیرفعال وجود ندارد.");
        }

        // ۲. آغاز تراکنش برای تضمین کسر نسخه و ثبت امانت به صورت همزمان
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // بارگذاری کتاب با پیگیری تغییرات (Tracking)
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                await transaction.RollbackAsync();
                return ServiceResult<Loan>.Fail("کتاب مورد نظر یافت نشد.");
            }

            // بررسی موجودی نسخه قابل امانت
            if (book.AvailableCopies <= 0)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning("درخواست امانت ناموفق: موجودی کتاب {Title} (ID: {BookId}) به اتمام رسیده است.", book.Title, book.Id);
                return ServiceResult<Loan>.Fail("متاسفانه هیچ نسخه‌ای از این کتاب در حال حاضر برای امانت موجود نیست.");
            }

            // ۳. ایجاد رکورد امانت
            var loan = new Loan
            {
                BookId = bookId,
                MemberId = memberId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(loanDays),
                Status = LoanStatus.Active,
                Notes = notes
            };

            // ۴. کسر یک واحد از موجودی آزاد کتاب
            book.AvailableCopies -= 1;
            book.UpdatedAt = DateTime.UtcNow;

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // ۵. ثبت قطعی تراکنش
            await transaction.CommitAsync();

            _logger.LogInformation("امانت کتاب {BookTitle} (ID: {BookId}) به عضو {MemberName} (ID: {MemberId}) با شناسه امانت {LoanId} ثبت شد.",
                book.Title, book.Id, member.FullName, member.Id, loan.Id);

            return ServiceResult<Loan>.Ok(loan, "امانت کتاب با موفقیت ثبت گردید.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "خطای سیستمی هنگام ثبت امانت کتاب {BookId} برای عضو {MemberId}", bookId, memberId);
            return ServiceResult<Loan>.Fail("خطایی در پردازش امانت رخ داد. عملیات لغو شد.");
        }
    }

    /// <summary>
    /// بازگرداندن کتاب به کتابخانه همراه با تراکنش اتمیک و افزایش موجودی
    /// </summary>
    public async Task<ServiceResult> ReturnLoanAsync(int loanId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
            {
                await transaction.RollbackAsync();
                return ServiceResult.Fail("رکورد امانت یافت نشد.");
            }

            if (loan.Status == LoanStatus.Returned || loan.ReturnDate != null)
            {
                await transaction.RollbackAsync();
                return ServiceResult.Fail("این کتاب قبلاً به کتابخانه تحویل داده شده است.");
            }

            // ثبت تاریخ تحویل
            loan.ReturnDate = DateTime.UtcNow;
            loan.Status = LoanStatus.Returned;

            // افزایش نسخه‌های در دسترس کتاب
            if (loan.Book != null)
            {
                if (loan.Book.AvailableCopies < loan.Book.TotalCopies)
                {
                    loan.Book.AvailableCopies += 1;
                }
                loan.Book.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("کتاب {BookTitle} با شناسه امانت {LoanId} توسط عضو {MemberName} بازگردانده شد.",
                loan.Book?.Title, loan.Id, loan.Member?.FullName);

            return ServiceResult.Ok("کتاب با موفقیت تحویل گرفته شد و موجودی قفسه به‌روز گردید.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "خطا در فرآیند بازگرداندن کتاب با شناسه امانت {LoanId}", loanId);
            return ServiceResult.Fail("خطا در ثبت بازگشت کتاب. عملیات بازگردانده شد.");
        }
    }

    /// <summary>
    /// تمدید مهلت امانت کتاب
    /// </summary>
    public async Task<ServiceResult> ExtendLoanAsync(int loanId, int additionalDays = 7)
    {
        if (additionalDays <= 0)
        {
            return ServiceResult.Fail("مدت زمان تمدید باید حداقل ۱ روز باشد.");
        }

        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
        {
            return ServiceResult.Fail("رکورد امانت یافت نشد.");
        }

        if (loan.Status == LoanStatus.Returned || loan.ReturnDate != null)
        {
            return ServiceResult.Fail("امکان تمدید کتابی که قبلاً تحویل داده شده است وجود ندارد.");
        }

        var baseDate = loan.DueDate > DateTime.UtcNow ? loan.DueDate : DateTime.UtcNow;
        loan.DueDate = baseDate.AddDays(additionalDays);
        loan.Status = LoanStatus.Active;

        await _context.SaveChangesAsync();
        _logger.LogInformation("مهلت امانت کتاب {BookTitle} برای عضو {MemberName} به مدت {Days} روز تمدید شد.",
            loan.Book?.Title, loan.Member?.FullName, additionalDays);

        return ServiceResult.Ok($"مهلت امانت کتاب با موفقیت به مدت {additionalDays} روز تمدید گردید.");
    }

    /// <summary>
    /// به‌روزرسانی دسته‌ای وضعیت امانت‌های دارای تاخیر به Overdue
    /// </summary>
    public async Task<int> UpdateOverdueStatusAsync()
    {
        var now = DateTime.UtcNow;
        var overdueLoans = await _context.Loans
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < now && l.ReturnDate == null)
            .ToListAsync();

        foreach (var loan in overdueLoans)
        {
            loan.Status = LoanStatus.Overdue;
        }

        if (overdueLoans.Any())
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("تعداد {Count} امانت به وضعیت تاخیر (Overdue) منتقل شدند.", overdueLoans.Count);
        }

        return overdueLoans.Count;
    }
}
