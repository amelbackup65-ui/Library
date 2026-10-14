namespace LibraryManagement.Models.Entities;

/// <summary>
/// موجودیت امانت کتاب به اعضا
/// </summary>
public class Loan
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public int MemberId { get; set; }

    /// <summary>
    /// تاریخ و زمان ثبت امانت
    /// </summary>
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// موعد مقرر جهت بازگرداندن کتاب به کتابخانه
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// تاریخ واقعی تحویل کتاب (در صورت بازگردانده نشدن مقدار null دارد)
    /// </summary>
    public DateTime? ReturnDate { get; set; }

    /// <summary>
    /// وضعیت جاری امانت: فعال، بازگردانده شده، یا دارای دیرکرد
    /// </summary>
    public LoanStatus Status { get; set; } = LoanStatus.Active;

    public string? Notes { get; set; }

    // Navigation Properties
    public virtual Book Book { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
