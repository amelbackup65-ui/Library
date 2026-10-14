using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);

        // ایندکس یکتا برای شابک جهت جلوگیری از ثبت تکراری و بالا بردن سرعت جستجو
        builder.HasIndex(b => b.ISBN)
            .IsUnique()
            .HasDatabaseName("IX_Books_ISBN");

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(b => b.Title)
            .HasDatabaseName("IX_Books_Title");

        builder.Property(b => b.Description)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(b => b.Publisher)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(b => b.PublishYear)
            .IsRequired();

        builder.Property(b => b.PageCount)
            .IsRequired();

        builder.Property(b => b.TotalCopies)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(b => b.AvailableCopies)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.UpdatedAt)
            .IsRequired(false);

        // رابطه با Category
        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
