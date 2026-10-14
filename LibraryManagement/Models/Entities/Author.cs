namespace LibraryManagement.Models.Entities;

/// <summary>
/// موجودیت نویسنده / مترجم / پدیدآورنده کتاب
/// </summary>
public class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Biography { get; set; }

    // فیلد کمکی فقط-خواندنی برای نمایش نام کامل نویسنده
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation Property: رابطه چند به چند با کتاب‌ها از طریق جدول واسط
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
