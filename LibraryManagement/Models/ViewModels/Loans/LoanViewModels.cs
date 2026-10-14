using System.ComponentModel.DataAnnotations;
using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Models.ViewModels.Loans;

public class LoanCreateViewModel
{
    [Required(ErrorMessage = "انتخاب کتاب الزامی است.")]
    [Display(Name = "کتاب مورد امانت")]
    public int BookId { get; set; }

    [Required(ErrorMessage = "انتخاب عضو الزامی است.")]
    [Display(Name = "عضو امانت‌گیرنده")]
    public int MemberId { get; set; }

    [Required(ErrorMessage = "مدت زمان امانت الزامی است.")]
    [Range(1, 90, ErrorMessage = "مدت زمان امانت باید بین ۱ تا ۹۰ روز باشد.")]
    [Display(Name = "مدت امانت (روز)")]
    public int LoanDays { get; set; } = 14;

    [Display(Name = "توضیحات یا یادداشت")]
    [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")]
    public string? Notes { get; set; }

    // لیست کتاب‌های دارای نسخه آماده امانت (AvailableCopies > 0)
    public SelectList? AvailableBookList { get; set; }

    // لیست اعضای فعال کتابخانه (IsActive == true)
    public SelectList? ActiveMemberList { get; set; }
}

public class LoanItemViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MemberPhone { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public string? Notes { get; set; }

    public bool IsOverdue => Status == LoanStatus.Overdue || (Status == LoanStatus.Active && DueDate < DateTime.UtcNow && ReturnDate == null);
    public int OverdueDays => IsOverdue ? (int)Math.Ceiling((DateTime.UtcNow - DueDate).TotalDays) : 0;
}

public class LoanListViewModel
{
    public PaginatedList<LoanItemViewModel> Loans { get; set; } = null!;
    public string? StatusFilter { get; set; }
    public string? SearchTerm { get; set; }
    public int ActiveLoansCount { get; set; }
    public int OverdueLoansCount { get; set; }
}

public class OverdueLoanItemViewModel
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string MemberPhone { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
}

public class LoanDetailsViewModel
{
    public int Id { get; set; }
    public Book Book { get; set; } = null!;
    public Member Member { get; set; } = null!;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public string? Notes { get; set; }
    public bool IsOverdue => Status == LoanStatus.Overdue || (Status == LoanStatus.Active && DueDate < DateTime.UtcNow && ReturnDate == null);
}
