using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("کاربر {User} گزارشات تحلیلی را مشاهده می‌کند.", User.Identity?.Name);
        var report = await _reportService.GetGeneralReportAsync();
        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> ExportLoansCsv()
    {
        _logger.LogInformation("کاربر {User} در حال دریافت فایل خروجی CSV امانات است.", User.Identity?.Name);
        var csvData = await _reportService.ExportLoansCsvAsync();
        var fileName = $"Loans_Report_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
        return File(csvData, "text/csv; charset=utf-8", fileName);
    }
}
