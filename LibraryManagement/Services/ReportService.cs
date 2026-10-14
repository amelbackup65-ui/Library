using System.Text;
using LibraryManagement.Data;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private const decimal DailyOverdueFine = 5000m; // ۵,۰۰۰ تومان جریمه روزانه تاخیر

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LibraryReportDto> GetGeneralReportAsync()
    {
        var totalBooks = await _context.Books.CountAsync();
        var totalCopies = await _context.Books.SumAsync(b => (int?)b.TotalCopies) ?? 0;
        var availableCopies = await _context.Books.SumAsync(b => (int?)b.AvailableCopies) ?? 0;
        var totalMembers = await _context.Members.CountAsync();

        var activeLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Active);
        var overdueLoansList = await _context.Loans
            .Where(l => l.Status == LoanStatus.Overdue || (l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow && l.ReturnDate == null))
            .ToListAsync();
        var overdueLoansCount = overdueLoansList.Count;

        var completedLoans = await _context.Loans.CountAsync(l => l.Status == LoanStatus.Returned);

        // محاسبه کل مبلغ تخمینی جریمه دیرکردها
        decimal totalFines = 0;
        var now = DateTime.UtcNow;
        foreach (var loan in overdueLoansList)
        {
            var overdueDays = (int)(now - loan.DueDate).TotalDays;
            if (overdueDays > 0)
            {
                totalFines += overdueDays * DailyOverdueFine;
            }
        }

        // پرامانت‌ترین کتاب‌ها (Top 5)
        var topBooks = await _context.Loans
            .GroupBy(l => l.BookId)
            .Select(g => new
            {
                BookId = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .Join(_context.Books.Include(b => b.Category),
                g => g.BookId,
                b => b.Id,
                (g, b) => new TopBorrowedBookDto(
                    b.Id,
                    b.Title,
                    b.ISBN,
                    b.Category != null ? b.Category.Name : "نامشخص",
                    g.Count
                ))
            .ToListAsync();

        // فعال‌ترین اعضای کتابخانه (Top 5)
        var topMembers = await _context.Loans
            .GroupBy(l => l.MemberId)
            .Select(g => new
            {
                MemberId = g.Key,
                TotalLoans = g.Count(),
                ActiveLoans = g.Count(x => x.Status == LoanStatus.Active)
            })
            .OrderByDescending(x => x.TotalLoans)
            .Take(5)
            .Join(_context.Members,
                g => g.MemberId,
                m => m.Id,
                (g, m) => new TopActiveMemberDto(
                    m.Id,
                    $"{m.FirstName} {m.LastName}",
                    m.NationalCode,
                    m.Phone,
                    g.TotalLoans,
                    g.ActiveLoans
                ))
            .ToListAsync();

        // وضعیت به تفکیک دسته‌بندی موضوعی
        var categoryStats = await _context.Categories
            .Select(c => new CategoryStatsDto(
                c.Name,
                c.Books.Count,
                c.Books.SelectMany(b => b.Loans).Count(l => l.Status == LoanStatus.Active)
            ))
            .ToListAsync();

        return new LibraryReportDto
        {
            TotalBooksCount = totalBooks,
            TotalCopiesCount = totalCopies,
            AvailableCopiesCount = availableCopies,
            TotalMembersCount = totalMembers,
            ActiveLoansCount = activeLoans,
            OverdueLoansCount = overdueLoansCount,
            TotalCompletedLoansCount = completedLoans,
            EstimatedTotalFines = totalFines,
            TopBooks = topBooks,
            TopMembers = topMembers,
            CategoryStats = categoryStats
        };
    }

    public async Task<byte[]> ExportLoansCsvAsync()
    {
        var loans = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();

        var sb = new StringBuilder();
        // هدر CSV به زبان فارسی
        sb.AppendLine("شناسه امانت,عنوان کتاب,شابک,نام عضو,کد ملی,شماره تلفن,تاریخ امانت,سررسید مقرر,تاریخ عودت,وضعیت,یادداشت");

        foreach (var loan in loans)
        {
            var statusStr = loan.Status switch
            {
                LoanStatus.Active => "جاری",
                LoanStatus.Returned => "تحویل شده",
                LoanStatus.Overdue => "دارای تاخیر",
                LoanStatus.Lost => "مفقود شده",
                _ => "نامشخص"
            };

            var loanDateStr = loan.LoanDate.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
            var dueDateStr = loan.DueDate.ToLocalTime().ToString("yyyy/MM/dd");
            var returnDateStr = loan.ReturnDate.HasValue ? loan.ReturnDate.Value.ToLocalTime().ToString("yyyy/MM/dd HH:mm") : "-";

            var cleanTitle = (loan.Book?.Title ?? "").Replace(",", "،");
            var cleanMember = (loan.Member?.FullName ?? "").Replace(",", "،");
            var cleanNotes = (loan.Notes ?? "").Replace(",", "،").Replace("\r", " ").Replace("\n", " ");

            sb.AppendLine($"{loan.Id},{cleanTitle},{loan.Book?.ISBN},{cleanMember},{loan.Member?.NationalCode},{loan.Member?.Phone},{loanDateStr},{dueDateStr},{returnDateStr},{statusStr},{cleanNotes}");
        }

        // افزودن UTF-8 BOM جهت نمایش صحیح حروف فارسی در نرم‌افزار Microsoft Excel
        var preamble = Encoding.UTF8.GetPreamble();
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        var result = new byte[preamble.Length + bytes.Length];
        Buffer.BlockCopy(preamble, 0, result, 0, preamble.Length);
        Buffer.BlockCopy(bytes, 0, result, preamble.Length, bytes.Length);

        return result;
    }
}
