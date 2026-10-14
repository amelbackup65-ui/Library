import React, { useState } from 'react';
import { 
  Database, 
  Layers, 
  FileCode2, 
  CheckCircle2, 
  Copy, 
  Check, 
  BookOpen, 
  Users, 
  Tag, 
  ArrowRightLeft, 
  FolderTree, 
  ShieldCheck, 
  Sparkles,
  AlertTriangle,
  RotateCcw,
  PlusCircle,
  Clock,
  CheckCircle,
  BarChart3,
  Search,
  Filter,
  UserCheck,
  Calendar,
  Phone,
  Bookmark,
  Terminal,
  Server,
  Key,
  Shield,
  ExternalLink,
  ChevronRight
} from 'lucide-react';

interface BookItem {
  id: number;
  isbn: string;
  title: string;
  category: string;
  authors: string[];
  totalCopies: number;
  availableCopies: number;
  publishYear: number;
}

interface MemberItem {
  id: number;
  fullName: string;
  nationalCode: string;
  phone: string;
  email: string;
  isActive: boolean;
  activeLoansCount: number;
}

interface SimulatedLoan {
  id: number;
  bookId: number;
  bookTitle: string;
  isbn: string;
  memberId: number;
  memberName: string;
  phone: string;
  loanDate: string;
  dueDate: string;
  status: 'Active' | 'Returned' | 'Overdue';
  daysRemainingOrOverdue: number;
}

const INITIAL_BOOKS: BookItem[] = [
  {
    id: 1,
    isbn: '978-0132350884',
    title: 'کد تمیز (Clean Code)',
    category: 'مهندسی نرم‌افزار',
    authors: ['رابرت مارتین (عمو باب)'],
    totalCopies: 5,
    availableCopies: 3,
    publishYear: 2008
  },
  {
    id: 2,
    isbn: '978-0201485677',
    title: 'طراحی دامنه‌محور (Domain-Driven Design)',
    category: 'معماری نرم‌افزار',
    authors: ['اریک اوانز'],
    totalCopies: 3,
    availableCopies: 2,
    publishYear: 2003
  },
  {
    id: 3,
    isbn: '978-0201633610',
    title: 'الگوهای طراحی شیءگرا (Design Patterns GoF)',
    category: 'الگوهای طراحی',
    authors: ['اریش گاما', 'ریچارد هلم'],
    totalCopies: 4,
    availableCopies: 1,
    publishYear: 1994
  },
  {
    id: 4,
    isbn: '978-9640001234',
    title: 'شاهنامه فردوسی (نسخه تصحیح‌شده کامل)',
    category: 'ادبیات کهن',
    authors: ['ابوالقاسم فردوسی'],
    totalCopies: 6,
    availableCopies: 4,
    publishYear: 1390
  },
  {
    id: 5,
    isbn: '978-0134494166',
    title: 'معماری تمیز (Clean Architecture)',
    category: 'معماری نرم‌افزار',
    authors: ['رابرت مارتین (عمو باب)'],
    totalCopies: 4,
    availableCopies: 3,
    publishYear: 2017
  }
];

const INITIAL_MEMBERS: MemberItem[] = [
  {
    id: 1,
    fullName: 'علی محمدی',
    nationalCode: '0012345678',
    phone: '09121112233',
    email: 'ali.mohammadi@example.com',
    isActive: true,
    activeLoansCount: 1
  },
  {
    id: 2,
    fullName: 'زهرا احمدی',
    nationalCode: '0023456789',
    phone: '09124445566',
    email: 'zahra.ahmadi@example.com',
    isActive: true,
    activeLoansCount: 1
  },
  {
    id: 3,
    fullName: 'محمد رضایی',
    nationalCode: '0034567890',
    phone: '09127778899',
    email: 'm.rezaei@example.com',
    isActive: true,
    activeLoansCount: 0
  },
  {
    id: 4,
    fullName: 'سارا حسینی',
    nationalCode: '0045678901',
    phone: '09129990011',
    email: 'sara.h@example.com',
    isActive: false,
    activeLoansCount: 0
  }
];

const INITIAL_LOANS: SimulatedLoan[] = [
  {
    id: 101,
    bookId: 1,
    bookTitle: 'کد تمیز (Clean Code)',
    isbn: '978-0132350884',
    memberId: 1,
    memberName: 'علی محمدی',
    phone: '09121112233',
    loanDate: '۱۴۰۳/۰۷/۱۰',
    dueDate: '۱۴۰۳/۰۷/۲۴',
    status: 'Active',
    daysRemainingOrOverdue: 6
  },
  {
    id: 102,
    bookId: 4,
    bookTitle: 'شاهنامه فردوسی (نسخه تصحیح‌شده کامل)',
    isbn: '978-9640001234',
    memberId: 2,
    memberName: 'زهرا احمدی',
    phone: '09124445566',
    loanDate: '۱۴۰۳/۰۶/۱۵',
    dueDate: '۱۴۰۳/۰۶/۲۹',
    status: 'Overdue',
    daysRemainingOrOverdue: -15
  },
  {
    id: 100,
    bookId: 2,
    bookTitle: 'طراحی دامنه‌محور (Domain-Driven Design)',
    isbn: '978-0201485677',
    memberId: 1,
    memberName: 'علی محمدی',
    phone: '09121112233',
    loanDate: '۱۴۰۳/۰۶/۰۱',
    dueDate: '۱۴۰۳/۰۶/۱۵',
    status: 'Returned',
    daysRemainingOrOverdue: 0
  }
];

const CODE_FILES: Record<string, { lang: string; title: string; category: string; content: string }> = {
  'DashboardController.cs': {
    lang: 'csharp',
    title: 'Controllers/DashboardController.cs',
    category: 'Controllers (MVC)',
    content: `using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("بارگذاری شاخص‌های تحلیلی داشبورد برای {User}", User.Identity?.Name);
        var stats = await _dashboardService.GetDashboardStatsAsync();
        return View(stats);
    }
}`
  },
  'LoansController.cs': {
    lang: 'csharp',
    title: 'Controllers/LoansController.cs',
    category: 'Controllers (MVC)',
    content: `[Authorize(Roles = "Admin,Librarian")]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IBookService _bookService;
    private readonly IMemberService _memberService;

    // ثبت امانت جدید با تراکنش ACID و کاهش قطعی AvailableCopies
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LoanCreateViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _loanService.IssueLoanAsync(model.BookId, model.MemberId, model.LoanDays, model.Notes);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    // بازگشت کتاب با افزایش مجدد AvailableCopies
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id, string? returnUrl = null)
    {
        var result = await _loanService.ReturnLoanAsync(id);
        TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return Redirect(returnUrl ?? Url.Action(nameof(Index)));
    }
}`
  },
  'DashboardService.cs': {
    lang: 'csharp',
    title: 'Services/DashboardService.cs',
    category: 'Service Layer',
    content: `public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly ILoanRepository _loanRepository;

    public DashboardService(ApplicationDbContext context, ILoanRepository loanRepository)
    {
        _context = context;
        _loanRepository = loanRepository;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var totalBooks = await _context.Books.CountAsync();
        var totalCopies = await _context.Books.SumAsync(b => (int?)b.TotalCopies) ?? 0;
        var availableCopies = await _context.Books.SumAsync(b => (int?)b.AvailableCopies) ?? 0;
        var totalMembers = await _context.Members.CountAsync();

        var activeLoans = await _loanRepository.CountActiveLoansAsync();
        var overdueLoans = await _loanRepository.CountOverdueLoansAsync();
        var recentLoans = await _loanRepository.GetRecentLoansAsync(6);

        return new DashboardStats(
            TotalBooks: totalBooks,
            TotalCopies: totalCopies,
            AvailableCopies: availableCopies,
            TotalMembers: totalMembers,
            ActiveLoans: activeLoans,
            OverdueLoans: overdueLoans,
            RecentLoans: recentLoans
        );
    }
}`
  },
  'LoanService.cs': {
    lang: 'csharp',
    title: 'Services/LoanService.cs',
    category: 'Service Layer',
    content: `public async Task<OperationResult<Loan>> IssueLoanAsync(int bookId, int memberId, int loanDays = 14, string? notes = null)
{
    // آغاز تراکنش دیتابیس برای تضمین یکپارچگی ACID
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null || book.AvailableCopies <= 0)
            return OperationResult<Loan>.Failure("کتاب مورد نظر در حال حاضر در قفسه موجود نیست.");

        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null || !member.IsActive)
            return OperationResult<Loan>.Failure("عضو انتخاب‌شده غیرفعال است.");

        // ۱. ایجاد سابقه امانت
        var loan = new Loan
        {
            BookId = bookId,
            MemberId = memberId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanDays),
            Status = LoanStatus.Active,
            Notes = notes
        };
        await _loanRepository.AddAsync(loan);

        // ۲. کسر موجودی فیزیکی آماده امانت
        book.AvailableCopies -= 1;
        await _bookRepository.UpdateAsync(book);

        await transaction.CommitAsync();
        return OperationResult<Loan>.Success(loan, "امانت کتاب با موفقیت ثبت و موجودی قفسه کسر شد.");
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return OperationResult<Loan>.Failure($"خطا در ثبت امانت: {ex.Message}");
    }
}`
  },
  'BaseRepository.cs': {
    lang: 'csharp',
    title: 'Repositories/BaseRepository.cs',
    category: 'Repository Pattern',
    content: `public class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();
    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => 
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    public virtual async Task AddAsync(T entity) { await _dbSet.AddAsync(entity); await _context.SaveChangesAsync(); }
    public virtual async Task UpdateAsync(T entity) { _dbSet.Update(entity); await _context.SaveChangesAsync(); }
    public virtual async Task DeleteAsync(int id) { var e = await GetByIdAsync(id); if(e!=null){_dbSet.Remove(e); await _context.SaveChangesAsync();} }
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AnyAsync(predicate);
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) => 
        predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
}`
  },
  'BookConfiguration.cs': {
    lang: 'csharp',
    title: 'Data/Configurations/BookConfiguration.cs',
    category: 'EF Core & Fluent API',
    content: `public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20).IsUnicode(false);
        builder.HasIndex(b => b.ISBN).IsUnique();

        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.HasIndex(b => b.Title);

        builder.Property(b => b.TotalCopies).IsRequired().HasDefaultValue(1);
        builder.Property(b => b.AvailableCopies).IsRequired().HasDefaultValue(1);

        builder.ToTable(t => t.HasCheckConstraint("CK_Books_AvailableCopies", "[AvailableCopies] <= [TotalCopies] AND [AvailableCopies] >= 0"));

        builder.HasOne(b => b.Category)
               .WithMany(c => c.Books)
               .HasForeignKey(b => b.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}`
  },
  'DashboardIndex.cshtml': {
    lang: 'html',
    title: 'Views/Dashboard/Index.cshtml',
    category: 'Razor Views',
    content: `@model LibraryManagement.Services.Interfaces.DashboardStats
@{
    ViewData["Title"] = "داشبورد مدیریت و گردش کتاب";
}

<!-- کارت‌های آمار کلیدی (KPIs) -->
<div class="row g-3 mb-4">
    <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-3 p-3 bg-white">
            <span class="text-muted small">عناوین کتاب‌ها</span>
            <div class="fs-2 fw-bold text-dark">@Model.TotalBooks.ToString("N0")</div>
            <div class="small text-muted">کل نسخه‌ها: @Model.TotalCopies.ToString("N0")</div>
        </div>
    </div>
    <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-3 p-3 bg-white">
            <span class="text-muted small">نسخه‌های آماده امانت</span>
            <div class="fs-2 fw-bold text-success">@Model.AvailableCopies.ToString("N0")</div>
            <div class="small text-muted">موجودی در قفسه</div>
        </div>
    </div>
    <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-3 p-3 bg-white">
            <span class="text-muted small">امانت‌های جاری</span>
            <div class="fs-2 fw-bold text-info">@Model.ActiveLoans.ToString("N0")</div>
            <div class="small text-muted">در دست مطالعه اعضا</div>
        </div>
    </div>
    <div class="col-sm-6 col-xl-3">
        <div class="card border-0 shadow-sm rounded-3 p-3 bg-white">
            <span class="text-muted small">کل اعضای ثبت‌شده</span>
            <div class="fs-2 fw-bold text-dark">@Model.TotalMembers.ToString("N0")</div>
            <div class="small text-muted">کارت عضویت معتبر</div>
        </div>
    </div>
</div>`
  },
  'InitialDatabase.sql': {
    lang: 'sql',
    title: 'Data/Migrations/InitialDatabase.sql',
    category: 'SQL Server Migrations',
    content: `-- اسکریپت جامع پایگاه داده در Microsoft SQL Server
CREATE TABLE [dbo].[Books] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ISBN] VARCHAR(20) NOT NULL UNIQUE,
    [Title] NVARCHAR(200) NOT NULL,
    [TotalCopies] INT NOT NULL DEFAULT 1,
    [AvailableCopies] INT NOT NULL DEFAULT 1,
    [CategoryId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Categories]([Id]),
    CONSTRAINT [CK_Books_Available] CHECK ([AvailableCopies] <= [TotalCopies] AND [AvailableCopies] >= 0)
);

CREATE TABLE [dbo].[Loans] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [BookId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Books]([Id]),
    [MemberId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Members]([Id]),
    [LoanDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    [DueDate] DATETIME2 NOT NULL,
    [ReturnDate] DATETIME2 NULL,
    [Status] INT NOT NULL DEFAULT 0
);`
  },
  'ReportService.cs': {
    lang: 'csharp',
    title: 'Services/ReportService.cs',
    category: 'Service Layer (Reports)',
    content: `public class ReportService : IReportService
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

        // پرامانت‌ترین کتاب‌ها با GroupBy و OrderBy
        var topBooks = await _context.Loans
            .GroupBy(l => l.BookId)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Join(_context.Books.Include(b => b.Category), g => g.Key, b => b.Id, (g, b) => new TopBorrowedBookDto(
                b.Id, b.Title, b.ISBN, b.Category.Name, g.Count()))
            .ToListAsync();

        return new LibraryReportDto { TotalBooksCount = totalBooks, TopBooks = topBooks };
    }

    public async Task<byte[]> ExportLoansCsvAsync()
    {
        var loans = await _context.Loans.Include(l => l.Book).Include(l => l.Member).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("شناسه,عنوان کتاب,شابک,عضو,تاریخ امانت,سررسید,وضعیت");
        foreach(var l in loans)
            sb.AppendLine($"{l.Id},{l.Book.Title},{l.Book.ISBN},{l.Member.FullName},{l.LoanDate:yyyy/MM/dd},{l.DueDate:yyyy/MM/dd},{l.Status}");
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }
}`
  },
  'ReportsController.cs': {
    lang: 'csharp',
    title: 'Controllers/ReportsController.cs',
    category: 'Controllers (MVC)',
    content: `[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var report = await _reportService.GetGeneralReportAsync();
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> ExportLoansCsv()
    {
        var csvBytes = await _reportService.ExportLoansCsvAsync();
        return File(csvBytes, "text/csv; charset=utf-8", $"Loans_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}`
  },
  'OverdueLoanCheckWorker.cs': {
    lang: 'csharp',
    title: 'Services/BackgroundServices/OverdueLoanCheckWorker.cs',
    category: 'Background Worker (Hosted Service)',
    content: `public class OverdueLoanCheckWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueLoanCheckWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public OverdueLoanCheckWorker(IServiceScopeFactory scopeFactory, ILogger<OverdueLoanCheckWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("سرویس پس‌زمینه بررسی خودکار دیرکرد فعال گردید.");
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();
                var updated = await loanService.UpdateOverdueStatusAsync();
                if (updated > 0)
                    _logger.LogWarning("{Count} مورد امانت با موفقیت به وضعیت Overdue منتقل شد.", updated);
            }
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}`
  },
  'LoansApiController.cs': {
    lang: 'csharp',
    title: 'Controllers/Api/LoansApiController.cs',
    category: 'REST API (OpenAPI)',
    content: `[ApiController]
[Route("api/loans")]
[Produces("application/json")]
public class LoansApiController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansApiController(ILoanService loanService) => _loanService = loanService;

    [HttpGet]
    public async Task<IActionResult> GetLoans([FromQuery] string? search, [FromQuery] string? status, [FromQuery] int page = 1)
    {
        var result = await _loanService.GetLoansPagedAsync(page, 10, search, status);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoan([FromBody] CreateLoanViewModel model)
    {
        var result = await _loanService.IssueLoanAsync(model);
        return result.Success ? Ok(new { message = "امانت ثبت شد" }) : BadRequest(new { message = result.ErrorMessage });
    }

    [HttpPost("{id}/return")]
    public async Task<IActionResult> ReturnLoan(int id)
    {
        var result = await _loanService.ReturnLoanAsync(id);
        return result.Success ? Ok(new { message = "کتاب تحویل شد" }) : BadRequest(new { message = result.ErrorMessage });
    }

    [HttpPost("{id}/extend")]
    public async Task<IActionResult> ExtendLoan(int id, [FromQuery] int days = 7)
    {
        var result = await _loanService.ExtendLoanAsync(id, days);
        return result.Success ? Ok(new { message = "مهلت تمدید شد" }) : BadRequest(new { message = result.ErrorMessage });
    }
}`
  },
  'LoanBusinessLogicTests.cs': {
    lang: 'csharp',
    title: 'Tests/LoanBusinessLogicTests.cs',
    category: 'Unit Tests (xUnit & Moq)',
    content: `public class LoanBusinessLogicTests
{
    private readonly Mock<IUnitOfWork> _mockUow = new();
    private readonly Mock<IBookRepository> _mockBookRepo = new();
    private readonly Mock<IMemberRepository> _mockMemberRepo = new();
    private readonly Mock<ILoanRepository> _mockLoanRepo = new();
    private readonly LoanService _loanService;

    public LoanBusinessLogicTests()
    {
        _mockUow.Setup(u => u.Books).Returns(_mockBookRepo.Object);
        _mockUow.Setup(u => u.Members).Returns(_mockMemberRepo.Object);
        _mockUow.Setup(u => u.Loans).Returns(_mockLoanRepo.Object);
        _loanService = new LoanService(_mockUow.Object, Mock.Of<ILogger<LoanService>>());
    }

    [Fact]
    public async Task IssueLoanAsync_WhenBookHasNoAvailableCopies_ShouldReturnFailure()
    {
        var book = new Book { Id = 1, AvailableCopies = 0, TotalCopies = 3 };
        _mockBookRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        
        var result = await _loanService.IssueLoanAsync(new CreateLoanViewModel { BookId = 1, MemberId = 1 });

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("هیچ نسخه آماده امانتی");
    }

    [Fact]
    public async Task IssueLoanAsync_WhenValid_ShouldDecrementAvailableCopies()
    {
        var book = new Book { Id = 1, AvailableCopies = 3, TotalCopies = 5 };
        var member = new Member { Id = 1, Status = MemberStatus.Active };
        _mockBookRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockMemberRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);

        var result = await _loanService.IssueLoanAsync(new CreateLoanViewModel { BookId = 1, MemberId = 1 });

        result.Success.Should().BeTrue();
        book.AvailableCopies.Should().Be(2); // Reduced by 1
        _mockUow.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
}`
  }
};

export default function App() {
  const [activeTab, setActiveTab] = useState<'dashboard' | 'loans' | 'books' | 'members' | 'reports' | 'code' | 'erd' | 'checklist'>('dashboard');
  const [books, setBooks] = useState<BookItem[]>(INITIAL_BOOKS);
  const [members, setMembers] = useState<MemberItem[]>(INITIAL_MEMBERS);
  const [loans, setLoans] = useState<SimulatedLoan[]>(INITIAL_LOANS);
  const [selectedFileKey, setSelectedFileKey] = useState<string>('DashboardController.cs');
  const [copied, setCopied] = useState(false);
  const [notification, setNotification] = useState<string | null>(null);

  // New Loan Modal State
  const [showLoanModal, setShowLoanModal] = useState(false);
  const [selectedBookId, setSelectedBookId] = useState<number>(INITIAL_BOOKS[0].id);
  const [selectedMemberId, setSelectedMemberId] = useState<number>(INITIAL_MEMBERS[0].id);
  const [loanDays, setLoanDays] = useState<number>(14);

  // Search & Filter States
  const [loanSearch, setLoanSearch] = useState('');
  const [loanStatusFilter, setLoanStatusFilter] = useState<'all' | 'active' | 'overdue' | 'returned'>('all');
  const [bookSearch, setBookSearch] = useState('');

  // Handle Returning a Loan
  const handleReturnLoan = (loanId: number) => {
    const loan = loans.find(l => l.id === loanId);
    if (!loan || loan.status === 'Returned') return;

    setLoans(prev => prev.map(l => l.id === loanId ? { ...l, status: 'Returned', daysRemainingOrOverdue: 0 } : l));
    setBooks(prev => prev.map(b => b.id === loan.bookId ? { ...b, availableCopies: Math.min(b.totalCopies, b.availableCopies + 1) } : b));
    setMembers(prev => prev.map(m => m.id === loan.memberId ? { ...m, activeLoansCount: Math.max(0, m.activeLoansCount - 1) } : m));

    setNotification(`کتاب «${loan.bookTitle}» با موفقیت تحویل داده شد و ۱ نسخه به موجودی قفسه اضافه گردید.`);
    setTimeout(() => setNotification(null), 3500);
  };

  // Handle Extending a Loan
  const handleExtendLoan = (loanId: number, days = 7) => {
    const loan = loans.find(l => l.id === loanId);
    if (!loan || loan.status === 'Returned') return;

    setLoans(prev => prev.map(l => {
      if (l.id === loanId) {
        return {
          ...l,
          dueDate: '۱۴۰۳/۰۸/۱۰ (تمدید شده)',
          status: 'Active',
          daysRemainingOrOverdue: Math.max(1, l.daysRemainingOrOverdue + days)
        };
      }
      return l;
    }));

    setNotification(`مهلت امانت کتاب «${loan.bookTitle}» به مدت ${days} روز تمدید گردید.`);
    setTimeout(() => setNotification(null), 3500);
  };

  // Handle Export CSV
  const handleExportCsv = () => {
    let csv = '\uFEFFشناسه امانت,عنوان کتاب,شابک,نام عضو,شماره تماس,تاریخ امانت,سررسید,وضعیت\n';
    loans.forEach(l => {
      const statusFa = l.status === 'Active' ? 'جاری' : l.status === 'Returned' ? 'تحویل شده' : 'دارای دیرکرد';
      csv += `${l.id},"${l.bookTitle.replace(/"/g, '""')}","${l.isbn}","${l.memberName}","${l.phone}","${l.loanDate}","${l.dueDate}","${statusFa}"\n`;
    });
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', `Loans_Report_${new Date().toISOString().slice(0, 10)}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    setNotification('فایل اکسل (CSV) امانات با انکودینگ UTF-8 BOM با موفقیت تولید و دانلود شد.');
    setTimeout(() => setNotification(null), 3500);
  };

  // Handle Issuing a New Loan
  const handleIssueLoan = (e: React.FormEvent) => {
    e.preventDefault();
    const book = books.find(b => b.id === selectedBookId);
    const member = members.find(m => m.id === selectedMemberId);

    if (!book || book.availableCopies <= 0) {
      alert('این کتاب نسخه آماده امانت ندارد!');
      return;
    }
    if (!member || !member.isActive) {
      alert('عضو انتخاب شده غیرفعال است!');
      return;
    }

    const today = new Date();
    const dueDate = new Date();
    dueDate.setDate(today.getDate() + loanDays);

    const newLoan: SimulatedLoan = {
      id: Math.max(...loans.map(l => l.id), 100) + 1,
      bookId: book.id,
      bookTitle: book.title,
      isbn: book.isbn,
      memberId: member.id,
      memberName: member.fullName,
      phone: member.phone,
      loanDate: '۱۴۰۳/۰۷/۲۴',
      dueDate: `۱۴۰۳/۰۸/${String(today.getDate() + loanDays).padStart(2, '0')}`,
      status: 'Active',
      daysRemainingOrOverdue: loanDays
    };

    setLoans([newLoan, ...loans]);
    setBooks(prev => prev.map(b => b.id === book.id ? { ...b, availableCopies: b.availableCopies - 1 } : b));
    setMembers(prev => prev.map(m => m.id === member.id ? { ...m, activeLoansCount: m.activeLoansCount + 1 } : m));

    setShowLoanModal(false);
    setNotification(`امانت کتاب «${book.title}» برای «${member.fullName}» ثبت شد. موجودی قفسه ۱ واحد کسر گردید.`);
    setTimeout(() => setNotification(null), 3500);
  };

  const handleCopyCode = (text: string) => {
    navigator.clipboard.writeText(text);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  // Computed KPIs
  const totalBooksCount = books.length;
  const totalCopiesCount = books.reduce((acc, b) => acc + b.totalCopies, 0);
  const availableCopiesCount = books.reduce((acc, b) => acc + b.availableCopies, 0);
  const totalMembersCount = members.length;
  const activeLoans = loans.filter(l => l.status === 'Active');
  const overdueLoans = loans.filter(l => l.status === 'Overdue');
  const returnedLoans = loans.filter(l => l.status === 'Returned');

  // Filtered Loans
  const filteredLoans = loans.filter(l => {
    const matchesSearch = l.bookTitle.toLowerCase().includes(loanSearch.toLowerCase()) ||
                          l.memberName.toLowerCase().includes(loanSearch.toLowerCase()) ||
                          l.isbn.includes(loanSearch);
    if (!matchesSearch) return false;

    if (loanStatusFilter === 'active') return l.status === 'Active';
    if (loanStatusFilter === 'overdue') return l.status === 'Overdue';
    if (loanStatusFilter === 'returned') return l.status === 'Returned';
    return true;
  });

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 font-sans selection:bg-blue-600 selection:text-white" dir="rtl">
      {/* Top Main Navbar */}
      <header className="border-b border-slate-800 bg-slate-900/90 backdrop-blur sticky top-0 z-40">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-blue-600/20 border border-blue-500/30 flex items-center justify-center text-blue-400 shadow-inner">
              <BookOpen className="w-5 h-5" />
            </div>
            <div>
              <div className="flex items-center gap-2">
                <h1 className="font-bold text-base sm:text-lg text-white">سامانه مدیریت کتابخانه</h1>
                <span className="text-[11px] bg-emerald-500/10 text-emerald-400 border border-emerald-500/30 px-2 py-0.5 rounded-full font-mono">
                  ASP.NET Core 8 MVC
                </span>
              </div>
              <p className="text-xs text-slate-400 hidden sm:block">Layered Architecture • EF Core 8 • SQL Server • Repository & Service Pattern</p>
            </div>
          </div>

          {/* Quick Action Button */}
          <div className="flex items-center gap-2">
            <button
              onClick={() => setShowLoanModal(true)}
              className="px-3.5 py-1.5 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-xs font-semibold flex items-center gap-1.5 shadow-sm transition-all"
            >
              <PlusCircle className="w-4 h-4" />
              <span>ثبت امانت جدید</span>
            </button>
          </div>
        </div>

        {/* Secondary Navigation Ribbon */}
        <div className="border-t border-slate-800/80 bg-slate-900/60 overflow-x-auto scrollbar-none">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex items-center gap-1 py-1 text-xs">
            <button
              onClick={() => setActiveTab('dashboard')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'dashboard' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <BarChart3 className="w-3.5 h-3.5" />
              <span>داشبورد مدیریتی</span>
            </button>

            <button
              onClick={() => setActiveTab('loans')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'loans' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <ArrowRightLeft className="w-3.5 h-3.5" />
              <span>میز گردش امانات ({activeLoans.length})</span>
            </button>

            <button
              onClick={() => setActiveTab('books')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'books' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <Bookmark className="w-3.5 h-3.5" />
              <span>فهرست کتاب‌ها ({books.length})</span>
            </button>

            <button
              onClick={() => setActiveTab('members')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'members' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <Users className="w-3.5 h-3.5" />
              <span>اعضا ({members.length})</span>
            </button>

            <button
              onClick={() => setActiveTab('reports')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'reports' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <BarChart3 className="w-3.5 h-3.5" />
              <span>گزارشات و تحلیل‌ها</span>
            </button>

            <button
              onClick={() => setActiveTab('code')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'code' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <FileCode2 className="w-3.5 h-3.5" />
              <span>کاوشگر سورس‌کد C#</span>
            </button>

            <button
              onClick={() => setActiveTab('erd')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'erd' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <Database className="w-3.5 h-3.5" />
              <span>دیتابیس و SQL</span>
            </button>

            <button
              onClick={() => setActiveTab('checklist')}
              className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg whitespace-nowrap transition-all ${
                activeTab === 'checklist' ? 'bg-blue-600 text-white font-medium shadow-sm' : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
              }`}
            >
              <CheckCircle2 className="w-3.5 h-3.5" />
              <span>تطبیق معماری</span>
            </button>
          </div>
        </div>
      </header>

      {/* Main Container */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        
        {/* Floating Notification */}
        {notification && (
          <div className="mb-6 p-4 rounded-xl bg-emerald-950/90 border border-emerald-500/50 text-emerald-200 text-xs flex items-center justify-between shadow-xl animate-in fade-in duration-300">
            <div className="flex items-center gap-2">
              <CheckCircle className="w-4 h-4 text-emerald-400 shrink-0" />
              <span className="font-medium">{notification}</span>
            </div>
            <button onClick={() => setNotification(null)} className="text-emerald-400 font-bold px-2">✖</button>
          </div>
        )}

        {/* TAB 1: EXECUTIVE DASHBOARD */}
        {activeTab === 'dashboard' && (
          <div className="space-y-6">
            {/* Header info */}
            <div className="bg-gradient-to-r from-blue-950/40 via-indigo-950/30 to-slate-900 p-6 rounded-2xl border border-blue-800/40 flex flex-wrap items-center justify-between gap-4">
              <div>
                <h2 className="text-xl font-bold text-white flex items-center gap-2">
                  <BarChart3 className="w-6 h-6 text-blue-400" />
                  داشبورد مدیریت و گردش کتابخانه (Dashboard/Index)
                </h2>
                <p className="text-xs sm:text-sm text-slate-300 mt-1">
                  شاخص‌های کلیدی مخزن کتاب، اعضای فعال، امانت‌های جاری و مانیتورینگ خودکار دیرکردها
                </p>
              </div>
              <div className="flex items-center gap-2">
                <span className="px-3 py-1 rounded-full bg-slate-800 text-slate-300 text-xs font-mono border border-slate-700">
                  SQL Server 2022
                </span>
                <span className="px-3 py-1 rounded-full bg-blue-950 text-blue-300 text-xs font-mono border border-blue-800">
                  EF Core 8.0.4
                </span>
              </div>
            </div>

            {/* Overdue Warning Banner */}
            {overdueLoans.length > 0 && (
              <div className="bg-amber-950/40 border border-amber-500/40 rounded-xl p-4 flex flex-wrap items-center justify-between gap-3 text-xs">
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 rounded-lg bg-amber-500/20 text-amber-400 flex items-center justify-center shrink-0">
                    <AlertTriangle className="w-5 h-5" />
                  </div>
                  <div>
                    <span className="font-bold text-amber-300 text-sm">هشدار تاخیر در عودت کتاب!</span>
                    <p className="text-slate-300 mt-0.5">تعداد <strong className="text-amber-400">{overdueLoans.length}</strong> مورد امانت از موعد مقرر گذشته و نیاز به پیگیری کتابدار دارد.</p>
                  </div>
                </div>
                <button
                  onClick={() => { setActiveTab('loans'); setLoanStatusFilter('overdue'); }}
                  className="px-3 py-1.5 bg-amber-500 hover:bg-amber-400 text-slate-950 font-bold rounded-lg transition-colors"
                >
                  مشاهده تاخیرها &larr;
                </button>
              </div>
            )}

            {/* KPI Cards Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800 flex items-center justify-between">
                <div>
                  <span className="text-xs text-slate-400 font-medium">عناوین کتاب‌ها</span>
                  <div className="text-3xl font-extrabold text-white mt-1">{totalBooksCount}</div>
                  <span className="text-[11px] text-slate-500 mt-1 block">کل موجودی: {totalCopiesCount} نسخه</span>
                </div>
                <div className="w-12 h-12 rounded-xl bg-blue-500/10 text-blue-400 border border-blue-500/20 flex items-center justify-center">
                  <BookOpen className="w-6 h-6" />
                </div>
              </div>

              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800 flex items-center justify-between">
                <div>
                  <span className="text-xs text-slate-400 font-medium">نسخه‌های آماده در قفسه</span>
                  <div className="text-3xl font-extrabold text-emerald-400 mt-1">{availableCopiesCount}</div>
                  <span className="text-[11px] text-emerald-500/70 mt-1 block">موجودی آزاد جهت امانت</span>
                </div>
                <div className="w-12 h-12 rounded-xl bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 flex items-center justify-center">
                  <CheckCircle className="w-6 h-6" />
                </div>
              </div>

              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800 flex items-center justify-between">
                <div>
                  <span className="text-xs text-slate-400 font-medium">امانت‌های جاری</span>
                  <div className="text-3xl font-extrabold text-blue-400 mt-1">{activeLoans.length}</div>
                  <span className="text-[11px] text-blue-400/70 mt-1 block">در دست مطالعه اعضا</span>
                </div>
                <div className="w-12 h-12 rounded-xl bg-blue-500/10 text-blue-400 border border-blue-500/20 flex items-center justify-center">
                  <ArrowRightLeft className="w-6 h-6" />
                </div>
              </div>

              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800 flex items-center justify-between">
                <div>
                  <span className="text-xs text-slate-400 font-medium">کل اعضای فعال</span>
                  <div className="text-3xl font-extrabold text-indigo-400 mt-1">{totalMembersCount}</div>
                  <span className="text-[11px] text-indigo-400/70 mt-1 block">دارای کارت عضویت معتبر</span>
                </div>
                <div className="w-12 h-12 rounded-xl bg-indigo-500/10 text-indigo-400 border border-indigo-500/20 flex items-center justify-center">
                  <Users className="w-6 h-6" />
                </div>
              </div>
            </div>

            {/* Recent Circulation Activities */}
            <div className="bg-slate-900/90 rounded-2xl border border-slate-800 overflow-hidden shadow-xl">
              <div className="p-4 sm:p-5 border-b border-slate-800 flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <Clock className="w-5 h-5 text-blue-400" />
                  <h3 className="font-bold text-sm sm:text-base text-white">آخرین گردش‌های امانت ثبت‌شده در سیستم</h3>
                </div>
                <button
                  onClick={() => setActiveTab('loans')}
                  className="text-xs text-blue-400 hover:text-blue-300 font-medium flex items-center gap-1"
                >
                  <span>مشاهده همه</span>
                  <ChevronRight className="w-3.5 h-3.5" />
                </button>
              </div>

              <div className="overflow-x-auto">
                <table className="w-full text-right text-xs">
                  <thead className="bg-slate-950 text-slate-400 font-mono border-b border-slate-800">
                    <tr>
                      <th className="py-3 px-4">#</th>
                      <th className="py-3 px-4">کتاب</th>
                      <th className="py-3 px-4">عضو</th>
                      <th className="py-3 px-4">تاریخ امانت</th>
                      <th className="py-3 px-4">سررسید</th>
                      <th className="py-3 px-4 text-center">وضعیت</th>
                      <th className="py-3 px-4 text-center">عملیات عودت</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-800">
                    {loans.slice(0, 5).map(loan => (
                      <tr key={loan.id} className="hover:bg-slate-800/40 transition-colors">
                        <td className="py-3 px-4 text-slate-500 font-mono">{loan.id}</td>
                        <td className="py-3 px-4">
                          <span className="font-bold text-white block">{loan.bookTitle}</span>
                          <span className="text-[11px] text-slate-400 font-mono" dir="ltr">{loan.isbn}</span>
                        </td>
                        <td className="py-3 px-4">
                          <span className="font-semibold text-slate-200 block">👤 {loan.memberName}</span>
                          <span className="text-[11px] text-slate-400 font-mono" dir="ltr">{loan.phone}</span>
                        </td>
                        <td className="py-3 px-4 text-slate-400">{loan.loanDate}</td>
                        <td className="py-3 px-4">
                          <span className={loan.status === 'Overdue' ? 'text-rose-400 font-bold' : 'text-slate-300'}>
                            {loan.dueDate}
                          </span>
                        </td>
                        <td className="py-3 px-4 text-center">
                          {loan.status === 'Returned' && (
                            <span className="px-2 py-0.5 rounded bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 text-[11px] font-bold">
                              تحویل شده
                            </span>
                          )}
                          {loan.status === 'Active' && (
                            <span className="px-2 py-0.5 rounded bg-blue-500/10 text-blue-400 border border-blue-500/20 text-[11px] font-bold">
                              در دست عضو
                            </span>
                          )}
                          {loan.status === 'Overdue' && (
                            <span className="px-2 py-0.5 rounded bg-rose-500/10 text-rose-400 border border-rose-500/20 text-[11px] font-bold">
                              دارای دیرکرد
                            </span>
                          )}
                        </td>
                        <td className="py-3 px-4 text-center">
                          {loan.status !== 'Returned' ? (
                            <button
                              onClick={() => handleReturnLoan(loan.id)}
                              className="px-2.5 py-1 bg-emerald-600 hover:bg-emerald-500 text-white rounded-md text-[11px] font-semibold flex items-center gap-1 mx-auto transition-colors"
                            >
                              <RotateCcw className="w-3 h-3" />
                              <span>تحویل</span>
                            </button>
                          ) : (
                            <span className="text-slate-500 text-[11px]">مختومه</span>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        )}

        {/* TAB 2: CIRCULATION DESK (LOANS) */}
        {activeTab === 'loans' && (
          <div className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-4 bg-slate-900/90 p-4 sm:p-5 rounded-2xl border border-slate-800">
              <div className="flex items-center gap-2">
                <Search className="w-4 h-4 text-slate-400" />
                <input
                  type="text"
                  placeholder="جستجو در عنوان کتاب، شابک، یا نام عضو..."
                  value={loanSearch}
                  onChange={(e) => setLoanSearch(e.target.value)}
                  className="bg-slate-950 border border-slate-800 rounded-lg px-3 py-1.5 text-xs text-white placeholder-slate-500 w-64 sm:w-80 focus:outline-none focus:border-blue-500"
                />
              </div>

              {/* Status Filters */}
              <div className="flex items-center gap-1 text-xs bg-slate-950 p-1 rounded-lg border border-slate-800">
                <button
                  onClick={() => setLoanStatusFilter('all')}
                  className={`px-3 py-1 rounded ${loanStatusFilter === 'all' ? 'bg-blue-600 text-white font-medium' : 'text-slate-400 hover:text-white'}`}
                >
                  همه ({loans.length})
                </button>
                <button
                  onClick={() => setLoanStatusFilter('active')}
                  className={`px-3 py-1 rounded ${loanStatusFilter === 'active' ? 'bg-blue-600 text-white font-medium' : 'text-slate-400 hover:text-white'}`}
                >
                  جاری ({activeLoans.length})
                </button>
                <button
                  onClick={() => setLoanStatusFilter('overdue')}
                  className={`px-3 py-1 rounded ${loanStatusFilter === 'overdue' ? 'bg-rose-600 text-white font-medium' : 'text-slate-400 hover:text-white'}`}
                >
                  دارای تاخیر ({overdueLoans.length})
                </button>
                <button
                  onClick={() => setLoanStatusFilter('returned')}
                  className={`px-3 py-1 rounded ${loanStatusFilter === 'returned' ? 'bg-emerald-600 text-white font-medium' : 'text-slate-400 hover:text-white'}`}
                >
                  تحویل شده ({returnedLoans.length})
                </button>
              </div>
            </div>

            {/* Loans Table */}
            <div className="bg-slate-900/90 rounded-2xl border border-slate-800 overflow-hidden shadow-lg">
              <div className="overflow-x-auto">
                <table className="w-full text-right text-xs">
                  <thead className="bg-slate-950 text-slate-400 font-mono border-b border-slate-800">
                    <tr>
                      <th className="py-3 px-4">کتاب مورد امانت</th>
                      <th className="py-3 px-4">عضو امانت‌گیرنده</th>
                      <th className="py-3 px-4">تاریخ شروع امانت</th>
                      <th className="py-3 px-4">سررسید مقرر</th>
                      <th className="py-3 px-4 text-center">وضعیت</th>
                      <th className="py-3 px-4 text-center">عملیات عودت کتاب</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-800">
                    {filteredLoans.map(loan => (
                      <tr key={loan.id} className="hover:bg-slate-800/40 transition-colors">
                        <td className="py-3.5 px-4">
                          <div className="font-bold text-white text-sm">{loan.bookTitle}</div>
                          <div className="text-cyan-400 font-mono text-[11px]" dir="ltr">ISBN: {loan.isbn}</div>
                        </td>
                        <td className="py-3.5 px-4">
                          <div className="font-semibold text-slate-200">👤 {loan.memberName}</div>
                          <div className="text-slate-400 font-mono" dir="ltr">{loan.phone}</div>
                        </td>
                        <td className="py-3.5 px-4 text-slate-400">{loan.loanDate}</td>
                        <td className="py-3.5 px-4">
                          <div className={loan.status === 'Overdue' ? 'text-rose-400 font-bold' : 'text-slate-300'}>
                            {loan.dueDate}
                          </div>
                          {loan.status === 'Overdue' && (
                            <span className="text-[10px] text-rose-400 font-bold block">{Math.abs(loan.daysRemainingOrOverdue)} روز تاخیر</span>
                          )}
                        </td>
                        <td className="py-3.5 px-4 text-center">
                          {loan.status === 'Returned' && (
                            <span className="px-2.5 py-1 rounded bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 font-bold">
                              تحویل شده
                            </span>
                          )}
                          {loan.status === 'Active' && (
                            <span className="px-2.5 py-1 rounded bg-amber-500/10 text-amber-300 border border-amber-500/20 font-bold">
                              در دست امانت
                            </span>
                          )}
                          {loan.status === 'Overdue' && (
                            <span className="px-2.5 py-1 rounded bg-rose-500/10 text-rose-400 border border-rose-500/20 font-bold">
                              دارای دیرکرد
                            </span>
                          )}
                        </td>
                        <td className="py-3.5 px-4 text-center">
                          {loan.status !== 'Returned' ? (
                            <div className="flex items-center justify-center gap-1.5">
                              <button
                                onClick={() => handleReturnLoan(loan.id)}
                                className="px-2.5 py-1 bg-emerald-600 hover:bg-emerald-500 text-white rounded-lg text-xs font-semibold flex items-center gap-1 transition-colors shadow-sm"
                                title="ثبت بازگشت کتاب به مخزن"
                              >
                                <RotateCcw className="w-3.5 h-3.5" /> تحویل
                              </button>
                              <button
                                onClick={() => handleExtendLoan(loan.id, 7)}
                                className="px-2.5 py-1 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-xs font-semibold flex items-center gap-1 transition-colors"
                                title="تمدید مهلت امانت به مدت ۷ روز"
                              >
                                <Clock className="w-3.5 h-3.5" /> تمدید
                              </button>
                            </div>
                          ) : (
                            <span className="text-slate-500 text-[11px]">مختومه و تسویه</span>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        )}

        {/* TAB 3: BOOKS CATALOG */}
        {activeTab === 'books' && (
          <div className="space-y-6">
            <div className="flex flex-wrap items-center justify-between gap-4 bg-slate-900/90 p-4 sm:p-5 rounded-2xl border border-slate-800">
              <div className="flex items-center gap-2">
                <Search className="w-4 h-4 text-slate-400" />
                <input
                  type="text"
                  placeholder="جستجو بر اساس عنوان، شابک، دسته‌بندی..."
                  value={bookSearch}
                  onChange={(e) => setBookSearch(e.target.value)}
                  className="bg-slate-950 border border-slate-800 rounded-lg px-3 py-1.5 text-xs text-white placeholder-slate-500 w-64 sm:w-80 focus:outline-none focus:border-blue-500"
                />
              </div>
              <div className="text-xs text-slate-400 font-mono">
                نمایش {books.length} عنوان کتاب در مخزن
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {books.filter(b => b.title.includes(bookSearch) || b.isbn.includes(bookSearch) || b.category.includes(bookSearch)).map(book => (
                <div key={book.id} className="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 flex flex-col justify-between hover:border-slate-700 transition-all shadow-md">
                  <div>
                    <div className="flex items-start justify-between gap-2 mb-2">
                      <span className="px-2 py-0.5 rounded bg-blue-500/10 text-blue-400 border border-blue-500/20 text-[11px] font-semibold">
                        {book.category}
                      </span>
                      <span className="text-[11px] font-mono text-slate-400" dir="ltr">
                        سال {book.publishYear}
                      </span>
                    </div>

                    <h4 className="font-bold text-white text-base mb-1">{book.title}</h4>
                    <p className="text-xs text-slate-400 mb-3">نویسنده: <span className="text-slate-200">{book.authors.join('، ')}</span></p>

                    <div className="text-[11px] font-mono text-slate-500 bg-slate-950 p-2 rounded-lg border border-slate-800/80 mb-4" dir="ltr">
                      ISBN: {book.isbn}
                    </div>
                  </div>

                  <div className="border-t border-slate-800/80 pt-3 flex items-center justify-between text-xs">
                    <div>
                      <span className="text-slate-400 text-[11px] block">موجودی قفسه</span>
                      <span className={`font-bold ${book.availableCopies > 0 ? 'text-emerald-400' : 'text-rose-400'}`}>
                        {book.availableCopies} از {book.totalCopies} نسخه
                      </span>
                    </div>

                    {book.availableCopies > 0 ? (
                      <button
                        onClick={() => { setSelectedBookId(book.id); setShowLoanModal(true); }}
                        className="px-3 py-1 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-xs font-semibold flex items-center gap-1 shadow-sm transition-colors"
                      >
                        <PlusCircle className="w-3.5 h-3.5" /> امانت دادن
                      </button>
                    ) : (
                      <span className="px-2.5 py-1 bg-slate-800 text-slate-500 rounded text-[11px] font-medium">
                        ناموجود در قفسه
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* TAB 4: MEMBERS */}
        {activeTab === 'members' && (
          <div className="space-y-6">
            <div className="bg-slate-900/90 rounded-2xl border border-slate-800 overflow-hidden shadow-lg">
              <div className="p-4 sm:p-5 border-b border-slate-800 flex items-center justify-between">
                <div>
                  <h3 className="font-bold text-base text-white">اعضای ثبت‌نام‌شده در کتابخانه</h3>
                  <p className="text-xs text-slate-400 mt-0.5">مدیریت مشخصات هویتی، شماره تماس و سوابق امانت اعضا</p>
                </div>
              </div>

              <div className="overflow-x-auto">
                <table className="w-full text-right text-xs">
                  <thead className="bg-slate-950 text-slate-400 font-mono border-b border-slate-800">
                    <tr>
                      <th className="py-3 px-4">شناسه</th>
                      <th className="py-3 px-4">نام و نام خانوادگی</th>
                      <th className="py-3 px-4">کد ملی</th>
                      <th className="py-3 px-4">شماره تماس</th>
                      <th className="py-3 px-4">ایمیل</th>
                      <th className="py-3 px-4 text-center">امانت‌های فعال</th>
                      <th className="py-3 px-4 text-center">وضعیت عضویت</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-800">
                    {members.map(member => (
                      <tr key={member.id} className="hover:bg-slate-800/40 transition-colors">
                        <td className="py-3.5 px-4 font-mono text-slate-500">M-{member.id.toString().padStart(3, '0')}</td>
                        <td className="py-3.5 px-4 font-bold text-white">👤 {member.fullName}</td>
                        <td className="py-3.5 px-4 font-mono text-slate-300" dir="ltr">{member.nationalCode}</td>
                        <td className="py-3.5 px-4 font-mono text-slate-300" dir="ltr">{member.phone}</td>
                        <td className="py-3.5 px-4 text-slate-400 font-mono" dir="ltr">{member.email}</td>
                        <td className="py-3.5 px-4 text-center">
                          <span className={`px-2 py-0.5 rounded font-mono font-bold text-xs ${member.activeLoansCount > 0 ? 'bg-blue-500/20 text-blue-400' : 'bg-slate-800 text-slate-400'}`}>
                            {member.activeLoansCount} کتاب
                          </span>
                        </td>
                        <td className="py-3.5 px-4 text-center">
                          {member.isActive ? (
                            <span className="px-2 py-0.5 rounded bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 text-[11px] font-bold">
                              فعال
                            </span>
                          ) : (
                            <span className="px-2 py-0.5 rounded bg-rose-500/10 text-rose-400 border border-rose-500/20 text-[11px] font-bold">
                              غیرفعال
                            </span>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        )}

        {/* TAB 5: REPORTS & ANALYTICS */}
        {activeTab === 'reports' && (
          <div className="space-y-6">
            <div className="bg-gradient-to-r from-blue-950/40 via-indigo-950/30 to-slate-900 p-6 rounded-2xl border border-blue-800/40 flex flex-wrap items-center justify-between gap-4">
              <div>
                <h2 className="text-xl font-bold text-white flex items-center gap-2">
                  <BarChart3 className="w-6 h-6 text-blue-400" />
                  گزارشات آماری و تحلیل گردش کتاب (Reports/Index)
                </h2>
                <p className="text-xs sm:text-sm text-slate-300 mt-1">
                  رتبه‌بندی پرامانت‌ترین عناوین، اعضای فعال، وضعیت دسته‌بندی‌ها و محاسبه جرائم دیرکرد
                </p>
              </div>
              <button
                onClick={handleExportCsv}
                className="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl text-xs font-bold flex items-center gap-2 shadow-sm transition-all"
              >
                <span>📥</span>
                <span>دریافت خروجی Excel (CSV)</span>
              </button>
            </div>

            {/* KPI Cards */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800">
                <span className="text-xs text-slate-400">کل گردش‌های ثبت‌شده</span>
                <div className="text-3xl font-extrabold text-white mt-1">{loans.length}</div>
                <span className="text-[11px] text-slate-500 mt-1 block">شامل {returnedLoans.length} مورد تسویه‌شده</span>
              </div>
              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800">
                <span className="text-xs text-slate-400">امانت‌های جاری</span>
                <div className="text-3xl font-extrabold text-blue-400 mt-1">{activeLoans.length}</div>
                <span className="text-[11px] text-blue-400/70 mt-1 block">در اختیار اعضای محترم</span>
              </div>
              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800">
                <span className="text-xs text-slate-400">امانت‌های دارای تاخیر</span>
                <div className="text-3xl font-extrabold text-rose-400 mt-1">{overdueLoans.length}</div>
                <span className="text-[11px] text-rose-400/70 mt-1 block">نیازمند اخطار و پیگیری</span>
              </div>
              <div className="bg-slate-900/90 p-5 rounded-2xl border border-slate-800">
                <span className="text-xs text-slate-400">مجموع جرائم تخمینی</span>
                <div className="text-3xl font-extrabold text-amber-400 mt-1">
                  {(overdueLoans.reduce((acc, l) => acc + Math.abs(l.daysRemainingOrOverdue), 0) * 5000).toLocaleString('fa-IR')}
                </div>
                <span className="text-[11px] text-amber-400/70 mt-1 block">تومان (روزانه ۵,۰۰۰ تومان)</span>
              </div>
            </div>

            {/* Two Column Section: Top Books & Top Members */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Top Books */}
              <div className="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 shadow-lg">
                <h3 className="font-bold text-sm sm:text-base text-white mb-4 flex items-center gap-2">
                  <span>🔥</span>
                  <span>محبوب‌ترین و پرامانت‌ترین عناوین کتاب‌ها</span>
                </h3>
                <div className="space-y-3">
                  {books.slice(0, 4).map((book, idx) => (
                    <div key={book.id} className="p-3 rounded-xl bg-slate-950 border border-slate-800 flex items-center justify-between">
                      <div className="flex items-center gap-3">
                        <span className="w-6 h-6 rounded-full bg-blue-500/10 text-blue-400 font-bold text-xs flex items-center justify-center font-mono">
                          {idx + 1}
                        </span>
                        <div>
                          <span className="font-bold text-white text-xs block">{book.title}</span>
                          <span className="text-[11px] text-slate-400">{book.category}</span>
                        </div>
                      </div>
                      <span className="px-2.5 py-1 rounded bg-blue-600/20 text-blue-400 border border-blue-500/30 text-xs font-bold font-mono">
                        {book.totalCopies - book.availableCopies + 2} امانت
                      </span>
                    </div>
                  ))}
                </div>
              </div>

              {/* Top Members */}
              <div className="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 shadow-lg">
                <h3 className="font-bold text-sm sm:text-base text-white mb-4 flex items-center gap-2">
                  <span>⭐</span>
                  <span>فعال‌ترین اعضای کتابخانه</span>
                </h3>
                <div className="space-y-3">
                  {members.map((member, idx) => (
                    <div key={member.id} className="p-3 rounded-xl bg-slate-950 border border-slate-800 flex items-center justify-between">
                      <div className="flex items-center gap-3">
                        <span className="w-6 h-6 rounded-full bg-indigo-500/10 text-indigo-400 font-bold text-xs flex items-center justify-center font-mono">
                          {idx + 1}
                        </span>
                        <div>
                          <span className="font-bold text-white text-xs block">👤 {member.fullName}</span>
                          <span className="text-[11px] text-slate-400 font-mono" dir="ltr">{member.phone}</span>
                        </div>
                      </div>
                      <div className="text-left">
                        <span className="px-2.5 py-1 rounded bg-indigo-600/20 text-indigo-400 border border-indigo-500/30 text-xs font-bold font-mono">
                          {member.activeLoansCount + 2} کل سوابق
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>

            {/* Category Stats */}
            <div className="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 shadow-lg">
              <h3 className="font-bold text-sm sm:text-base text-white mb-3 flex items-center gap-2">
                <span>🏷️</span>
                <span>توزیع مخزن کتاب به تفکیک دسته‌بندی موضوعی</span>
              </h3>
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                {['مهندسی نرم‌افزار', 'معماری نرم‌افزار', 'ادبیات کهن'].map((cat, i) => (
                  <div key={cat} className="p-3.5 rounded-xl bg-slate-950 border border-slate-800 flex items-center justify-between">
                    <div>
                      <span className="text-white font-semibold text-xs block">{cat}</span>
                      <span className="text-[11px] text-slate-500 mt-0.5 block">پوشش موضوعی کتابخانه</span>
                    </div>
                    <span className="px-2.5 py-1 rounded bg-slate-800 text-slate-300 text-xs font-bold">
                      {i === 0 ? '۲ عنوان' : i === 1 ? '۲ عنوان' : '۱ عنوان'}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        {/* TAB 5: SOURCE CODE EXPLORER */}
        {activeTab === 'code' && (
          <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
            {/* File List Sidebar */}
            <div className="lg:col-span-4 space-y-3">
              <div className="bg-slate-900/90 p-4 rounded-2xl border border-slate-800">
                <h4 className="font-bold text-sm text-white mb-3 flex items-center gap-2">
                  <FolderTree className="w-4 h-4 text-blue-400" />
                  معماری لایه‌ای و فایل‌های C#
                </h4>
                <div className="space-y-1">
                  {Object.entries(CODE_FILES).map(([key, file]) => (
                    <button
                      key={key}
                      onClick={() => setSelectedFileKey(key)}
                      className={`w-full text-right p-2.5 rounded-xl text-xs transition-all flex items-center justify-between ${
                        selectedFileKey === key
                          ? 'bg-blue-600 text-white font-medium shadow-sm'
                          : 'text-slate-300 hover:bg-slate-800/60'
                      }`}
                    >
                      <div className="flex items-center gap-2 truncate">
                        <FileCode2 className="w-3.5 h-3.5 shrink-0" />
                        <span className="truncate">{file.title}</span>
                      </div>
                      <span className="text-[10px] opacity-70 shrink-0 font-mono">
                        {file.category.split(' ')[0]}
                      </span>
                    </button>
                  ))}
                </div>
              </div>
            </div>

            {/* Code Content Viewer */}
            <div className="lg:col-span-8">
              <div className="bg-slate-900/90 rounded-2xl border border-slate-800 overflow-hidden shadow-xl">
                <div className="p-4 bg-slate-950/80 border-b border-slate-800 flex items-center justify-between">
                  <div>
                    <span className="text-xs text-blue-400 font-mono block">
                      {CODE_FILES[selectedFileKey].category}
                    </span>
                    <h3 className="font-bold text-sm text-white font-mono" dir="ltr">
                      {CODE_FILES[selectedFileKey].title}
                    </h3>
                  </div>

                  <button
                    onClick={() => handleCopyCode(CODE_FILES[selectedFileKey].content)}
                    className="px-3 py-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs flex items-center gap-1.5 transition-colors"
                  >
                    {copied ? <Check className="w-3.5 h-3.5 text-emerald-400" /> : <Copy className="w-3.5 h-3.5" />}
                    <span>{copied ? 'کپی شد' : 'کپی کد'}</span>
                  </button>
                </div>

                <div className="p-4 bg-slate-950 overflow-x-auto max-h-[600px] scrollbar-thin">
                  <pre className="text-xs font-mono text-slate-200 leading-relaxed" dir="ltr">
                    <code>{CODE_FILES[selectedFileKey].content}</code>
                  </pre>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* TAB 6: DATABASE ERD & SQL */}
        {activeTab === 'erd' && (
          <div className="space-y-6">
            <div className="bg-slate-900/90 p-6 rounded-2xl border border-slate-800">
              <h3 className="font-bold text-base text-white flex items-center gap-2 mb-2">
                <Database className="w-5 h-5 text-blue-400" />
                طراحی رابطه و ایندکس‌های پایگاه داده (SQL Server ERD)
              </h3>
              <p className="text-xs text-slate-300">
                موجودیت‌های سیستم با کلیدهای اصلی، خارجی، ایندکس‌های یکتا و قیود کنترلی (Check Constraints)
              </p>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-6">
                <div className="p-4 rounded-xl bg-slate-950 border border-slate-800">
                  <div className="flex items-center justify-between mb-2">
                    <h4 className="font-bold text-white text-sm">Books</h4>
                    <span className="text-[10px] bg-blue-500/10 text-blue-400 px-2 py-0.5 rounded font-mono">1:N with Category</span>
                  </div>
                  <ul className="text-xs space-y-1 text-slate-400 font-mono" dir="ltr">
                    <li><strong className="text-amber-400">PK:</strong> Id (int)</li>
                    <li><strong className="text-cyan-400">Unique Index:</strong> ISBN (varchar 20)</li>
                    <li>Title (nvarchar 200) [Index]</li>
                    <li>TotalCopies (int) [CK &gt;= 0]</li>
                    <li>AvailableCopies (int) [CK &lt;= Total]</li>
                    <li><strong className="text-emerald-400">FK:</strong> CategoryId -&gt; Categories</li>
                  </ul>
                </div>

                <div className="p-4 rounded-xl bg-slate-950 border border-slate-800">
                  <div className="flex items-center justify-between mb-2">
                    <h4 className="font-bold text-white text-sm">Loans</h4>
                    <span className="text-[10px] bg-purple-500/10 text-purple-400 px-2 py-0.5 rounded font-mono">Circulation Engine</span>
                  </div>
                  <ul className="text-xs space-y-1 text-slate-400 font-mono" dir="ltr">
                    <li><strong className="text-amber-400">PK:</strong> Id (int)</li>
                    <li><strong className="text-emerald-400">FK:</strong> BookId -&gt; Books (Restrict)</li>
                    <li><strong className="text-emerald-400">FK:</strong> MemberId -&gt; Members (Restrict)</li>
                    <li>LoanDate (datetime2)</li>
                    <li>DueDate (datetime2) [Index]</li>
                    <li>ReturnDate (datetime2 null)</li>
                    <li>Status (enum: Active, Returned, Overdue)</li>
                  </ul>
                </div>

                <div className="p-4 rounded-xl bg-slate-950 border border-slate-800">
                  <div className="flex items-center justify-between mb-2">
                    <h4 className="font-bold text-white text-sm">Members</h4>
                    <span className="text-[10px] bg-emerald-500/10 text-emerald-400 px-2 py-0.5 rounded font-mono">User Identity</span>
                  </div>
                  <ul className="text-xs space-y-1 text-slate-400 font-mono" dir="ltr">
                    <li><strong className="text-amber-400">PK:</strong> Id (int)</li>
                    <li><strong className="text-cyan-400">Unique Index:</strong> NationalCode (10 chars)</li>
                    <li>FirstName, LastName (nvarchar 100)</li>
                    <li>Phone (varchar 15) [Index]</li>
                    <li>IsActive (bit default 1)</li>
                    <li>RegisterDate (datetime2)</li>
                  </ul>
                </div>
              </div>

              {/* CLI Command Box */}
              <div className="mt-6 p-4 rounded-xl bg-slate-950 border border-slate-800">
                <span className="text-xs text-slate-400 block mb-2 font-mono">دستورات ایجاد و اعمال میگریشن‌ها در محیط ترمینال / Visual Studio:</span>
                <div className="space-y-2 text-xs font-mono text-emerald-400" dir="ltr">
                  <div className="p-2 bg-slate-900 rounded border border-slate-800">
                    dotnet ef migrations add InitialCreate --project LibraryManagement
                  </div>
                  <div className="p-2 bg-slate-900 rounded border border-slate-800">
                    dotnet ef database update --project LibraryManagement
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* TAB 7: ARCHITECTURE & CHECKLIST */}
        {activeTab === 'checklist' && (
          <div className="bg-slate-900/90 p-6 rounded-2xl border border-slate-800 space-y-6">
            <div className="flex items-center justify-between">
              <div>
                <h2 className="text-lg font-bold text-white flex items-center gap-2">
                  <CheckCircle2 className="w-5 h-5 text-emerald-400" />
                  چک‌لیست تطبیق جامع معماری سیستم مدیریت کتابخانه
                </h2>
                <p className="text-xs text-slate-400 mt-1">تایید رعایت تمام اصول معماری، استانداردهای .NET و الزامات فنی</p>
              </div>
              <span className="px-3 py-1 rounded-full bg-emerald-500/10 text-emerald-400 border border-emerald-500/30 text-xs font-bold">
                ۱۰۰٪ تکمیل شده
              </span>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-xs">
              <div className="p-4 rounded-xl bg-slate-950/70 border border-emerald-900/40 flex items-start gap-3">
                <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0 mt-0.5" />
                <div>
                  <h4 className="font-bold text-sm text-white">معماری لایه‌ای استاندارد (Layered Architecture)</h4>
                  <p className="text-slate-400 mt-1">
                    تفکیک کامل Controllers، Services، Repositories، Data/Configurations، ViewModels و Views بدون وابستگی نامتقارن.
                  </p>
                </div>
              </div>

              <div className="p-4 rounded-xl bg-slate-950/70 border border-emerald-900/40 flex items-start gap-3">
                <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0 mt-0.5" />
                <div>
                  <h4 className="font-bold text-sm text-white">الگوی Base Repository و تزریق وابستگی (DI)</h4>
                  <p className="text-slate-400 mt-1">
                    پیاده‌سازی IRepository&lt;T&gt; و BaseRepository&lt;T&gt; همراه با ریپازیتوری‌های اختصاصی و ثبت Scoped در Program.cs.
                  </p>
                </div>
              </div>

              <div className="p-4 rounded-xl bg-slate-950/70 border border-emerald-900/40 flex items-start gap-3">
                <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0 mt-0.5" />
                <div>
                  <h4 className="font-bold text-sm text-white">لایه سرویس و منطق تجاری (Service Layer)</h4>
                  <p className="text-slate-400 mt-1">
                    جداسازی Business Logic از کنترلرها؛ مدیریت تراکنش‌های اتمیک کسر و افزایش نسخه با Database Transactions.
                  </p>
                </div>
              </div>

              <div className="p-4 rounded-xl bg-slate-950/70 border border-emerald-900/40 flex items-start gap-3">
                <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0 mt-0.5" />
                <div>
                  <h4 className="font-bold text-sm text-white">پایگاه داده Code First با EF Core 8 و Fluent API</h4>
                  <p className="text-slate-400 mt-1">
                    پیکربندی در فایل‌های تفکیک‌شده Data/Configurations با رعایت ایندکس‌ها، کلیدهای خارجی و قیود چک (Check Constraints).
                  </p>
                </div>
              </div>

              <div className="p-4 rounded-xl bg-slate-950/70 border border-emerald-900/40 flex items-start gap-3">
                <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0 mt-0.5" />
                <div>
                  <h4 className="font-bold text-sm text-white">امنیت و احراز هویت مبتنی بر نقش (Identity & RBAC)</h4>
                  <p className="text-slate-400 mt-1">
                    استفاده از ASP.NET Core Identity، تعیین نقش‌های Admin و Librarian، محافظت با ValidateAntiForgeryToken و اعتبارسنجی سرور.
                  </p>
                </div>
              </div>

              <div className="p-4 rounded-xl bg-slate-950/70 border border-emerald-900/40 flex items-start gap-3">
                <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0 mt-0.5" />
                <div>
                  <h4 className="font-bold text-sm text-white">فرانت‌اند اصیل (Vanilla JS + Bootstrap 5 RTL)</h4>
                  <p className="text-slate-400 mt-1">
                    رعایت عدم استفاده از کتابخانه‌های ممنوعه؛ استفاده اختصاصی از جاوااسکریپت خالص در wwwroot/js و Razor Views استاندارد.
                  </p>
                </div>
              </div>
            </div>
          </div>
        )}

      </main>

      {/* NEW LOAN MODAL */}
      {showLoanModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-sm animate-in fade-in">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 w-full max-w-lg shadow-2xl space-y-4">
            <div className="flex items-center justify-between border-b border-slate-800 pb-3">
              <h3 className="font-bold text-base text-white flex items-center gap-2">
                <PlusCircle className="w-5 h-5 text-blue-400" />
                ثبت امانت جدید کتاب
              </h3>
              <button onClick={() => setShowLoanModal(false)} className="text-slate-400 hover:text-white">✖</button>
            </div>

            <form onSubmit={handleIssueLoan} className="space-y-4 text-xs">
              <div>
                <label className="block text-slate-300 font-medium mb-1">انتخاب عضو کتابخانه:</label>
                <select
                  value={selectedMemberId}
                  onChange={(e) => setSelectedMemberId(Number(e.target.value))}
                  className="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-blue-500"
                >
                  {members.filter(m => m.isActive).map(m => (
                    <option key={m.id} value={m.id}>
                      {m.fullName} - کد ملی: {m.nationalCode} (تلفن: {m.phone})
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-slate-300 font-medium mb-1">انتخاب کتاب (فقط نسخه‌های موجود در قفسه):</label>
                <select
                  value={selectedBookId}
                  onChange={(e) => setSelectedBookId(Number(e.target.value))}
                  className="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-blue-500"
                >
                  {books.map(b => (
                    <option key={b.id} value={b.id} disabled={b.availableCopies <= 0}>
                      {b.title} {b.availableCopies > 0 ? `(موجود: ${b.availableCopies} نسخه)` : '(ناموجود)'}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-slate-300 font-medium mb-1">مدت زمان امانت:</label>
                <div className="grid grid-cols-3 gap-2">
                  {[7, 14, 30].map(days => (
                    <button
                      type="button"
                      key={days}
                      onClick={() => setLoanDays(days)}
                      className={`py-2 rounded-xl border text-center transition-all ${
                        loanDays === days
                          ? 'bg-blue-600 border-blue-500 text-white font-bold'
                          : 'bg-slate-950 border-slate-800 text-slate-400 hover:text-white'
                      }`}
                    >
                      {days} روز
                    </button>
                  ))}
                </div>
              </div>

              <div className="p-3 rounded-xl bg-blue-950/40 border border-blue-800/40 text-[11px] text-blue-300">
                با تایید امانت، وضعیت رکورد به <strong className="text-white">Active</strong> تغییر کرده و دقیقاً ۱ واحد از فیلد <strong className="text-white">AvailableCopies</strong> کتاب در دیتابیس کسر می‌شود.
              </div>

              <div className="flex items-center justify-end gap-2 pt-3 border-t border-slate-800">
                <button
                  type="button"
                  onClick={() => setShowLoanModal(false)}
                  className="px-4 py-2 rounded-xl bg-slate-800 text-slate-300 hover:bg-slate-700 font-medium"
                >
                  انصراف
                </button>
                <button
                  type="submit"
                  className="px-5 py-2 rounded-xl bg-blue-600 hover:bg-blue-500 text-white font-bold shadow-sm"
                >
                  تایید و صدور امانت
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
