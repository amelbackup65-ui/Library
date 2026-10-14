using LibraryManagement.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Services.BackgroundServices;

/// <summary>
/// سرویس پس‌زمینه (Hosted Service) جهت بررسی خودکار و دوره‌ای امانات دارای دیرکرد
/// و به‌روزرسانی وضعیت آن‌ها به Overdue در دیتابیس
/// </summary>
public class OverdueLoanCheckWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueLoanCheckWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // هر ۱ ساعت یکبار

    public OverdueLoanCheckWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<OverdueLoanCheckWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("سرویس بررسی خودکار دیرکرد امانات فعال گردید.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطایی در حین اجرای سرویس پس‌زمینه بررسی دیرکرد رخ داد.");
            }

            // انتظار تا دور بعد با در نظر گرفتن لغو عملیات
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("سرویس بررسی خودکار دیرکرد امانات متوقف گردید.");
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();

        _logger.LogInformation("در حال بررسی وضعیت امانات منقضی شده در پایگاه داده...");
        var updatedCount = await loanService.UpdateOverdueStatusAsync();

        if (updatedCount > 0)
        {
            _logger.LogWarning("تعداد {Count} مورد امانت با موفقیت به وضعیت 'دارای تاخیر' (Overdue) منتقل شدند.", updatedCount);
        }
        else
        {
            _logger.LogInformation("هیچ امانت جدیدی با موعد منقضی شده یافت نشد.");
        }
    }
}
