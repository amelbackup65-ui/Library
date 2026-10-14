using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.ViewModels.Authors;

public class AuthorFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "ورود نام نویسنده الزامی است.")]
    [StringLength(100, ErrorMessage = "نام نویسنده نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    [Display(Name = "نام")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ورود نام خانوادگی نویسنده الزامی است.")]
    [StringLength(100, ErrorMessage = "نام خانوادگی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    [Display(Name = "نام خانوادگی")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "زندگی‌نامه و سوابق")]
    [StringLength(2000, ErrorMessage = "زندگی‌نامه نمی‌تواند بیشتر از ۲۰۰۰ کاراکتر باشد.")]
    public string? Biography { get; set; }
}

public class AuthorItemViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public int BooksCount { get; set; }
}

public class AuthorDetailsViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public List<AuthorBookItemViewModel> Books { get; set; } = new();
}

public class AuthorBookItemViewModel
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int PublishYear { get; set; }
    public int AvailableCopies { get; set; }
}
