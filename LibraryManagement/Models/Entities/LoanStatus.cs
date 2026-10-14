namespace LibraryManagement.Models.Entities;

/// <summary>
/// وضعیت‌های مجاز برای یک درخواست یا رکورد امانت کتاب
/// </summary>
public enum LoanStatus
{
    /// <summary>
    /// امانت فعال بوده و در دست امانت‌گیرنده است
    /// </summary>
    Active = 1,

    /// <summary>
    /// کتاب با موفقیت تحویل کتابخانه داده شده است
    /// </summary>
    Returned = 2,

    /// <summary>
    /// موعد برگشت سپری شده و هنوز برگردانده نشده است
    /// </summary>
    Overdue = 3
}
