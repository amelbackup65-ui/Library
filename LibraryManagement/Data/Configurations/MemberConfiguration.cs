using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(m => m.LastName)
            .HasDatabaseName("IX_Members_LastName");

        // کد ملی ۱۰ رقمی با ایندکس یکتا
        builder.Property(m => m.NationalCode)
            .IsRequired()
            .HasMaxLength(10)
            .IsFixedLength()
            .IsUnicode(false);

        builder.HasIndex(m => m.NationalCode)
            .IsUnique()
            .HasDatabaseName("IX_Members_NationalCode");

        // شماره تلفن
        builder.Property(m => m.Phone)
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.HasIndex(m => m.Phone)
            .HasDatabaseName("IX_Members_Phone");

        builder.Property(m => m.Email)
            .HasMaxLength(150)
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(m => m.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(m => m.RegisterDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Ignore(m => m.FullName);
    }
}
