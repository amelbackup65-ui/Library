using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Models.ViewModels.Books;

public class BookEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ورود شماره شابک (ISBN) الزامی است.")]
    [StringLength(20, ErrorMessage = "شابک نمی‌تواند بیشتر از ۲۰ کاراکتر باشد.")]
    [Display(Name = "شماره شابک (ISBN)")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "ورود عنوان کتاب الزامی است.")]
    [StringLength(250, ErrorMessage = "عنوان کتاب نمی‌تواند بیشتر از ۲۵۰ کاراکتر باشد.")]
    [Display(Name = "عنوان کتاب")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "توضیحات و خلاصه")]
    [StringLength(2000, ErrorMessage = "توضیحات نمی‌تواند بیشتر از ۲۰۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }

    [Display(Name = "ناشر")]
    [StringLength(150, ErrorMessage = "نام ناشر نمی‌تواند بیشتر از ۱۵۰ کاراکتر باشد.")]
    public string? Publisher { get; set; }

    [Required(ErrorMessage = "سال انتشار الزامی است.")]
    [Range(1000, 2100, ErrorMessage = "سال انتشار معتبر نمی‌باشد.")]
    [Display(Name = "سال انتشار")]
    public int PublishYear { get; set; }

    [Required(ErrorMessage = "تعداد صفحات الزامی است.")]
    [Range(1, 10000, ErrorMessage = "تعداد صفحات باید حداقل ۱ باشد.")]
    [Display(Name = "تعداد صفحات")]
    public int PageCount { get; set; }

    [Required(ErrorMessage = "تعداد کل نسخه‌ها الزامی است.")]
    [Range(1, 1000, ErrorMessage = "تعداد نسخه‌ها باید حداقل ۱ باشد.")]
    [Display(Name = "تعداد کل نسخه‌ها")]
    public int TotalCopies { get; set; }

    [Display(Name = "نسخه‌های موجود در قفسه")]
    public int AvailableCopies { get; set; }

    [Required(ErrorMessage = "انتخاب دسته‌بندی موضوعی الزامی است.")]
    [Display(Name = "دسته‌بندی")]
    public int CategoryId { get; set; }

    [Display(Name = "نویسندگان")]
    public List<int> SelectedAuthorIds { get; set; } = new();

    public SelectList? CategoryList { get; set; }
    public MultiSelectList? AuthorList { get; set; }
}
