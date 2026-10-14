namespace LibraryManagement.Models.Entities;

/// <summary>
/// موجودیت اصلی کتاب در سیستم کتابخانه
/// </summary>
public class Book
{
    public int Id { get; set; }

    /// <summary>
    /// شماره استاندارد بین‌المللی کتاب (ISBN) - یکتا در سطح پایگاه داده
    /// </summary>
    public string ISBN { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Publisher { get; set; }

    public int PublishYear { get; set; }

    public int PageCount { get; set; }

    /// <summary>
    /// کل نسخه‌های فیزیکی خریداری شده/موجود در سامانه
    /// </summary>
    public int TotalCopies { get; set; }

    /// <summary>
    /// نسخه‌های آزاد فعلی در قفسه‌ها که آماده امانت‌دهی هستند
    /// </summary>
    public int AvailableCopies { get; set; }

    /// <summary>
    /// شناسه دسته‌بندی موضوعی
    /// </summary>
    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
