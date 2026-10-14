using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Models.Entities;

/// <summary>
/// موجودیت کاربر سیستم که از IdentityUser ارث‌بری می‌کند
/// جهت ورود و احراز هویت کتابداران و مدیران سامانه
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
