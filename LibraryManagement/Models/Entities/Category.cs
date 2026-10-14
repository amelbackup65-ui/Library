namespace LibraryManagement.Models.Entities;

/// <summary>
/// موجودیت دسته‌بندی موضوعی کتاب‌ها
/// </summary>
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation Property: رابطه یک به چند با کتاب‌ها
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
