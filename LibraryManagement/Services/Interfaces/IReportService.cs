namespace LibraryManagement.Services.Interfaces;

public record TopBorrowedBookDto(int BookId, string Title, string ISBN, string CategoryName, int BorrowCount);
public record TopActiveMemberDto(int MemberId, string FullName, string NationalCode, string Phone, int TotalLoansCount, int ActiveLoansCount);
public record CategoryStatsDto(string CategoryName, int BooksCount, int ActiveLoansCount);

public class LibraryReportDto
{
    public int TotalBooksCount { get; set; }
    public int TotalCopiesCount { get; set; }
    public int AvailableCopiesCount { get; set; }
    public int TotalMembersCount { get; set; }
    public int ActiveLoansCount { get; set; }
    public int OverdueLoansCount { get; set; }
    public int TotalCompletedLoansCount { get; set; }
    public decimal EstimatedTotalFines { get; set; } // مبلغ تخمینی جرائم تاخیر (ریال / تومان)

    public IEnumerable<TopBorrowedBookDto> TopBooks { get; set; } = Enumerable.Empty<TopBorrowedBookDto>();
    public IEnumerable<TopActiveMemberDto> TopMembers { get; set; } = Enumerable.Empty<TopActiveMemberDto>();
    public IEnumerable<CategoryStatsDto> CategoryStats { get; set; } = Enumerable.Empty<CategoryStatsDto>();
}

public interface IReportService
{
    Task<LibraryReportDto> GetGeneralReportAsync();
    Task<byte[]> ExportLoansCsvAsync();
}
