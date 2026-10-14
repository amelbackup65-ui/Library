using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.LoanDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(l => l.DueDate)
            .IsRequired();

        builder.Property(l => l.ReturnDate)
            .IsRequired(false);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<int>();

        // ایندکس ترکیبی روی وضعیت و تاریخ سررسید جهت کوئری‌های سریع Overdue و فعال
        builder.HasIndex(l => new { l.Status, l.DueDate })
            .HasDatabaseName("IX_Loans_Status_DueDate");

        builder.Property(l => l.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        // رابطه با کتاب
        builder.HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // رابطه با عضو
        builder.HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
