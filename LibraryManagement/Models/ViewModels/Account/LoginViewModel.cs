using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.ViewModels.Account;

/// <summary>
/// ویومدل صفحه ورود به سیستم برای کتابداران و مدیران
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "لطفاً نام کاربری را وارد نمایید.")]
    [Display(Name = "نام کاربری")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "لطفاً کلمه عبور را وارد نمایید.")]
    [DataType(DataType.Password)]
    [Display(Name = "کلمه عبور")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
