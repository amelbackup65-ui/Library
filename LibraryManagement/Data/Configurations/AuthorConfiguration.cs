using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(a => a.LastName)
            .HasDatabaseName("IX_Authors_LastName");

        builder.Property(a => a.Biography)
            .HasMaxLength(2000)
            .IsRequired(false);

        // فیلد محاسباتی نادیده گرفته شود تا در دیتابیس ستون ایجاد نکند
        builder.Ignore(a => a.FullName);
    }
}
