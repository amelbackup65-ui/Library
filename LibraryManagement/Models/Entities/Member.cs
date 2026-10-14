namespace LibraryManagement.Models.Entities;

/// <summary>
/// موجودیت عضو کتابخانه
/// </summary>
public class Member
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// کد ملی ده رقمی یکتای عضو
    /// </summary>
    public string NationalCode { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime RegisterDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // فیلد کمکی فقط-خواندنی
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation Property: سوابق امانت‌های عضو
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
