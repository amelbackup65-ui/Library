using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.ViewModels.Categories;

public class CategoryFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "ورود نام دسته‌بندی الزامی است.")]
    [StringLength(100, ErrorMessage = "نام دسته‌بندی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    [Display(Name = "نام دسته‌بندی")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "توضیحات موضوعی")]
    [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }
}

public class CategoryItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int BooksCount { get; set; }
}
