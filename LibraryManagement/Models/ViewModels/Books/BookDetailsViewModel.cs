using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.ViewModels.Books;

public class BookDetailsViewModel
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public int PublishYear { get; set; }
    public int PageCount { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public int LoanedCopies => TotalCopies - AvailableCopies;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<Author> Authors { get; set; } = new();
    public List<Loan> LoansHistory { get; set; } = new();
}
