namespace LibraryManagement.Models.Entities;

/// <summary>
/// جدول واسط (Join Entity) برای رابطه چند-به-چند بین کتاب‌ها و نویسندگان
/// دارای کلید ترکیبی (Composite Primary Key: BookId + AuthorId)
/// </summary>
public class BookAuthor
{
    public int BookId { get; set; }
    public virtual Book Book { get; set; } = null!;

    public int AuthorId { get; set; }
    public virtual Author Author { get; set; } = null!;
}
