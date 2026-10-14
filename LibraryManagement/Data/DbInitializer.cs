using LibraryManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data;

/// <summary>
/// راه‌اندازی و تزریق داده‌های اولیه (Seed Data) در دیتابیس
/// </summary>
public static class DbInitializer
{
    public const string RoleAdmin = "Admin";
    public const string RoleLibrarian = "Librarian";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        // ۱. اطمینان از اعمال میگریشن‌ها
        await context.Database.MigrateAsync();

        // ۲. ایجاد نقش‌های سیستمی (Roles)
        string[] roles = { RoleAdmin, RoleLibrarian };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("نقش امنیتی {Role} در سیستم ایجاد شد.", role);
            }
        }

        // ۳. ایجاد کاربر مدیر سیستم (Admin)
        var adminUserName = "admin";
        var adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = "admin@library.local",
                FullName = "مدیر ارشد سامانه",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, RoleAdmin);
                logger.LogInformation("کاربر مدیر سیستم (admin) با موفقیت ایجاد گردید.");
            }
        }

        // ۴. ایجاد کاربر کتابدار سیستم (Librarian)
        var librarianUserName = "librarian";
        var librarianUser = await userManager.FindByNameAsync(librarianUserName);
        if (librarianUser == null)
        {
            librarianUser = new ApplicationUser
            {
                UserName = librarianUserName,
                Email = "librarian@library.local",
                FullName = "کتابدار مسئول",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(librarianUser, RoleLibrarian);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(librarianUser, RoleLibrarian);
                logger.LogInformation("کاربر کتابدار (librarian) با موفقیت ایجاد گردید.");
            }
        }

        // ۵. تزریق داده‌های پایه دسته‌بندی‌ها (Categories)
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "مهندسی نرم‌افزار و کامپیوتر", Description = "کتب معماری نرم‌افزار، برنامه‌نویسی و پایگاه داده" },
                new() { Name = "ادبیات و شعر فارسی", Description = "آثار برجسته ادبیات کهن و معاصر ایران" },
                new() { Name = "روانشناسی و علوم تربیتی", Description = "مهارت‌های فردی، شناختی و جامعه‌شناسی" },
                new() { Name = "تاریخ و فلسفه", Description = "بررسی رویدادهای تاریخی و مکاتب فلسفی" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            logger.LogInformation("دسته‌بندی‌های اولیه با موفقیت ثبت شدند.");
        }

        // ۶. تزریق داده‌های اولیه نویسندگان (Authors)
        if (!await context.Authors.AnyAsync())
        {
            var authors = new List<Author>
            {
                new() { FirstName = "مارتین", LastName = "فاولر", Biography = "نویسنده نامدار حوزه معماری نرم‌افزار و ریفکتورینگ" },
                new() { FirstName = "رابرت", LastName = "مارتین (عمو باب)", Biography = "بنیان‌گذار اصول تمیزنویسی کد و تفکر چابک" },
                new() { FirstName = "ابوالقاسم", LastName = "فردوسی", Biography = "حماسه‌سرای بزرگ تاریخ ایران و سرآفرین شاهنامه" },
                new() { FirstName = "ویکتور", LastName = "هوگو", Biography = "ادیب و نویسنده شهیر فرانسوی قرن نوزدهم" }
            };

            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
            logger.LogInformation("نویسندگان اولیه با موفقیت ثبت شدند.");
        }

        // ۷. تزریق داده‌های اولیه کتاب‌ها (Books)
        if (!await context.Books.AnyAsync())
        {
            var itCategory = await context.Categories.FirstAsync(c => c.Name.Contains("نرم‌افزار"));
            var litCategory = await context.Categories.FirstAsync(c => c.Name.Contains("ادبیات"));

            var authorFowler = await context.Authors.FirstAsync(a => a.LastName == "فاولر");
            var authorMartin = await context.Authors.FirstAsync(a => a.LastName.Contains("مارتین"));
            var authorFerdowsi = await context.Authors.FirstAsync(a => a.LastName == "فردوسی");

            var book1 = new Book
            {
                ISBN = "978-0132350884",
                Title = "کد تمیز (Clean Code)",
                Publisher = "Prentice Hall",
                PublishYear = 2008,
                PageCount = 464,
                TotalCopies = 5,
                AvailableCopies = 5,
                CategoryId = itCategory.Id,
                Description = "راهنمای جامع شیوه‌نامه‌های تمیزنویسی و توسعه چابک نرم‌افزار",
                CreatedAt = DateTime.UtcNow
            };

            var book2 = new Book
            {
                ISBN = "978-0201485677",
                Title = "ریفکتورینگ (Refactoring)",
                Publisher = "Addison-Wesley",
                PublishYear = 2018,
                PageCount = 448,
                TotalCopies = 3,
                AvailableCopies = 3,
                CategoryId = itCategory.Id,
                Description = "بهبود ساختار و معماری کدهای موجود بدون تغییر رفتار بیرونی",
                CreatedAt = DateTime.UtcNow
            };

            var book3 = new Book
            {
                ISBN = "978-9640001234",
                Title = "شاهنامه فردوسی (نسخه کامل)",
                Publisher = "انتشارات علمی و فرهنگی",
                PublishYear = 2020,
                PageCount = 1250,
                TotalCopies = 4,
                AvailableCopies = 4,
                CategoryId = litCategory.Id,
                Description = "شاهکار بی‌بدیل حماسی زبان فارسی و یادگار تاریخ اساطیری ایران",
                CreatedAt = DateTime.UtcNow
            };

            await context.Books.AddRangeAsync(book1, book2, book3);
            await context.SaveChangesAsync();

            // ارتباط با نویسندگان
            await context.BookAuthors.AddRangeAsync(
                new BookAuthor { BookId = book1.Id, AuthorId = authorMartin.Id },
                new BookAuthor { BookId = book2.Id, AuthorId = authorFowler.Id },
                new BookAuthor { BookId = book3.Id, AuthorId = authorFerdowsi.Id }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("کتاب‌های اولیه و اتصالات نویسندگان ثبت شدند.");
        }

        // ۸. تزریق داده‌های اولیه اعضای کتابخانه (Members)
        if (!await context.Members.AnyAsync())
        {
            var members = new List<Member>
            {
                new()
                {
                    FirstName = "علی",
                    LastName = "محمدی",
                    NationalCode = "0012345678",
                    Phone = "09121112233",
                    Email = "ali.mohammadi@example.com",
                    Address = "تهران، خیابان آزادی، کوچه نیلوفر",
                    RegisterDate = DateTime.UtcNow.AddMonths(-3),
                    IsActive = true
                },
                new()
                {
                    FirstName = "زهرا",
                    LastName = "احمدی",
                    NationalCode = "0087654321",
                    Phone = "09124445566",
                    Email = "zahra.ahmadi@example.com",
                    Address = "تهران، میدان انقلاب، پلاک ۲۰",
                    RegisterDate = DateTime.UtcNow.AddMonths(-1),
                    IsActive = true
                }
            };

            await context.Members.AddRangeAsync(members);
            await context.SaveChangesAsync();
            logger.LogInformation("اعضای نمونه در پایگاه داده ایجاد شدند.");
        }
    }
}
