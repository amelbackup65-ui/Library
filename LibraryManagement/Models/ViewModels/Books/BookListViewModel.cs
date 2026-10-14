using LibraryManagement.Models.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Models.ViewModels.Books;

public class BookItemViewModel
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public int PublishYear { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string AuthorsDisplay { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public bool CanBeBorrowed => AvailableCopies > 0;
}

public class BookListViewModel
{
    public PaginatedList<BookItemViewModel> Books { get; set; } = null!;
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public string? SortOrder { get; set; }
    public SelectList? CategoryList { get; set; }
}
