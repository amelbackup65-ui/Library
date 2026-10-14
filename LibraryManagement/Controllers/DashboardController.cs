using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

/// <summary>
/// کنترلر داشبورد مدیریتی جهت نمایش شاخص‌های آماری و فعالیت‌های اخیر سامانه
/// </summary>
[Authorize(Roles = "Admin,Librarian")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("کاربر {User} در حال مشاهده صفحه داشبورد مدیریتی است.", User.Identity?.Name);
        var stats = await _dashboardService.GetDashboardStatsAsync();
        return View(stats);
    }
}
