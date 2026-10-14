using System.ComponentModel.DataAnnotations;
using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Common;

namespace LibraryManagement.Models.ViewModels.Members;

public class MemberFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "ورود نام الزامی است.")]
    [StringLength(100, ErrorMessage = "نام نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    [Display(Name = "نام")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ورود نام خانوادگی الزامی است.")]
    [StringLength(100, ErrorMessage = "نام خانوادگی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ورود کد ملی الزامی است.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید دقیقاً ۱۰ رقم باشد.")]
    [Display(Name = "کد ملی (۱۰ رقم)")]
    public string NationalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "ورود شماره تلفن الزامی است.")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل باید با ۰۹ شروع شده و ۱۱ رقم باشد (مثال: 09121112233).")]
    [Display(Name = "شماره تماس / موبایل")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "فرمت آدرس ایمیل نامعتبر است.")]
    [StringLength(150, ErrorMessage = "ایمیل نمی‌تواند بیشتر از ۱۵۰ کاراکتر باشد.")]
    [Display(Name = "پست الکترونیک")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "آدرس نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")]
    [Display(Name = "آدرس سکونت")]
    public string? Address { get; set; }

    [Display(Name = "وضعیت عضویت (فعال / غیرفعال)")]
    public bool IsActive { get; set; } = true;
}

public class MemberItemViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime RegisterDate { get; set; }
    public bool IsActive { get; set; }
    public int ActiveLoansCount { get; set; }
}

public class MemberListViewModel
{
    public PaginatedList<MemberItemViewModel> Members { get; set; } = null!;
    public string? SearchTerm { get; set; }
    public string? StatusFilter { get; set; }
}

public class MemberDetailsViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime RegisterDate { get; set; }
    public bool IsActive { get; set; }
    public int ActiveLoansCount { get; set; }
    public List<Loan> LoansHistory { get; set; } = new();
}
