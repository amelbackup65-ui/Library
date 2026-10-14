using FluentAssertions;
using LibraryManagement.Models.Entities;
using LibraryManagement.Models.ViewModels.Loans;
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LibraryManagement.Tests;

public class LoanBusinessLogicTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IBookRepository> _mockBookRepo;
    private readonly Mock<IMemberRepository> _mockMemberRepo;
    private readonly Mock<ILoanRepository> _mockLoanRepo;
    private readonly Mock<ILogger<LoanService>> _mockLogger;
    private readonly LoanService _loanService;

    public LoanBusinessLogicTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockBookRepo = new Mock<IBookRepository>();
        _mockMemberRepo = new Mock<IMemberRepository>();
        _mockLoanRepo = new Mock<ILoanRepository>();
        _mockLogger = new Mock<ILogger<LoanService>>();

        _mockUow.Setup(u => u.Books).Returns(_mockBookRepo.Object);
        _mockUow.Setup(u => u.Members).Returns(_mockMemberRepo.Object);
        _mockUow.Setup(u => u.Loans).Returns(_mockLoanRepo.Object);

        _loanService = new LoanService(_mockUow.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task IssueLoanAsync_WhenBookHasNoAvailableCopies_ShouldReturnFailure()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Clean Architecture", AvailableCopies = 0, TotalCopies = 3 };
        var member = new Member { Id = 1, FullName = "سارا امینی", Status = MemberStatus.Active };

        _mockBookRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockMemberRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);

        var model = new CreateLoanViewModel { BookId = 1, MemberId = 1, LoanDays = 14 };

        // Act
        var result = await _loanService.IssueLoanAsync(model);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("هیچ نسخه آماده امانتی");
        _mockUow.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task IssueLoanAsync_WhenMemberIsInactive_ShouldReturnFailure()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Clean Architecture", AvailableCopies = 2, TotalCopies = 3 };
        var member = new Member { Id = 1, FullName = "کاربر غیرفعال", Status = MemberStatus.Inactive };

        _mockBookRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockMemberRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);

        var model = new CreateLoanViewModel { BookId = 1, MemberId = 1, LoanDays = 14 };

        // Act
        var result = await _loanService.IssueLoanAsync(model);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("عضویت فعال ندارد");
        _mockUow.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task IssueLoanAsync_WhenValid_ShouldDecrementAvailableCopiesAndCommit()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Domain-Driven Design", AvailableCopies = 3, TotalCopies = 5 };
        var member = new Member { Id = 1, FullName = "علی رضایی", Status = MemberStatus.Active };

        _mockBookRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockMemberRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
        _mockLoanRepo.Setup(r => r.CountActiveLoansByMemberAsync(1)).ReturnsAsync(0);

        var model = new CreateLoanViewModel { BookId = 1, MemberId = 1, LoanDays = 14 };

        // Act
        var result = await _loanService.IssueLoanAsync(model);

        // Assert
        result.Success.Should().BeTrue();
        book.AvailableCopies.Should().Be(2); // Reduced from 3 to 2
        _mockUow.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task ReturnLoanAsync_WhenActive_ShouldIncrementAvailableCopiesAndSetReturned()
    {
        // Arrange
        var book = new Book { Id = 10, Title = "Design Patterns", AvailableCopies = 1, TotalCopies = 4 };
        var loan = new Loan 
        { 
            Id = 50, 
            BookId = 10, 
            Book = book, 
            MemberId = 1, 
            Status = LoanStatus.Active 
        };

        _mockLoanRepo.Setup(r => r.GetByIdWithDetailsAsync(50)).ReturnsAsync(loan);
        _mockBookRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(book);

        // Act
        var result = await _loanService.ReturnLoanAsync(50);

        // Assert
        result.Success.Should().BeTrue();
        loan.Status.Should().Be(LoanStatus.Returned);
        loan.ReturnDate.Should().NotBeNull();
        book.AvailableCopies.Should().Be(2); // Increased from 1 to 2
        _mockUow.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task ExtendLoanAsync_WhenValid_ShouldExtendDueDate()
    {
        // Arrange
        var originalDueDate = DateTime.UtcNow.AddDays(2);
        var loan = new Loan 
        { 
            Id = 100, 
            Status = LoanStatus.Active, 
            DueDate = originalDueDate 
        };

        _mockLoanRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(loan);

        // Act
        var result = await _loanService.ExtendLoanAsync(100, 7);

        // Assert
        result.Success.Should().BeTrue();
        loan.DueDate.Should().BeCloseTo(originalDueDate.AddDays(7), precision: TimeSpan.FromSeconds(2));
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
