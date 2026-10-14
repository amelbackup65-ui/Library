using LibraryManagement.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data;

/// <summary>
/// کانتکست اصلی پایگاه داده مبتنی بر EF Core و Identity
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories => Set<Category>();
    public virtual DbSet<Author> Authors => Set<Author>();
    public virtual DbSet<Book> Books => Set<Book>();
    public virtual DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public virtual DbSet<Member> Members => Set<Member>();
    public virtual DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // فراخوانی کانفیگ‌های پیش‌فرض Identity (نظیر جدول‌های AspNetUsers, AspNetRoles و ...)
        base.OnModelCreating(modelBuilder);

        // اعمال خودکار تمام فایل‌های پیکربندی IEntityTypeConfiguration در این اسمبلی
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
