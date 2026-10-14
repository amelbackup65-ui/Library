-- ==========================================================
-- سامانه مدیریت کتابخانه (Library Management System)
-- اسکریپت جامع ایجاد ساختار پایگاه داده در Microsoft SQL Server
-- مطابق با مدل Code-First و تنظیمات Entity Framework Core 8
-- ==========================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LibraryManagementDb')
BEGIN
    CREATE DATABASE LibraryManagementDb COLLATE Persian_100_CI_AI;
END
GO

USE LibraryManagementDb;
GO

-- 1. جدول دسته‌بندی‌ها (Categories)
IF OBJECT_ID(N'[dbo].[Categories]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_Name] ON [dbo].[Categories]([Name] ASC);
END
GO

-- 2. جدول نویسندگان (Authors)
IF OBJECT_ID(N'[dbo].[Authors]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Authors] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Biography] NVARCHAR(2000) NULL,
        CONSTRAINT [PK_Authors] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE NONCLUSTERED INDEX [IX_Authors_LastName_FirstName] ON [dbo].[Authors]([LastName] ASC, [FirstName] ASC);
END
GO

-- 3. جدول کتاب‌ها (Books)
IF OBJECT_ID(N'[dbo].[Books]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Books] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ISBN] VARCHAR(20) NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(2000) NULL,
        [Publisher] NVARCHAR(150) NULL,
        [PublishYear] INT NOT NULL,
        [PageCount] INT NOT NULL,
        [TotalCopies] INT NOT NULL DEFAULT 1,
        [AvailableCopies] INT NOT NULL DEFAULT 1,
        [CategoryId] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [PK_Books] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Books_Categories_CategoryId] FOREIGN KEY ([CategoryId]) 
            REFERENCES [dbo].[Categories] ([Id]) ON DELETE RESTRICT,
        CONSTRAINT [CK_Books_AvailableCopies] CHECK ([AvailableCopies] <= [TotalCopies] AND [AvailableCopies] >= 0),
        CONSTRAINT [CK_Books_TotalCopies] CHECK ([TotalCopies] >= 0)
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Books_ISBN] ON [dbo].[Books]([ISBN] ASC);
    CREATE NONCLUSTERED INDEX [IX_Books_Title] ON [dbo].[Books]([Title] ASC);
    CREATE NONCLUSTERED INDEX [IX_Books_CategoryId] ON [dbo].[Books]([CategoryId] ASC);
END
GO

-- 4. جدول واسط کتاب و نویسنده (BookAuthors)
IF OBJECT_ID(N'[dbo].[BookAuthors]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BookAuthors] (
        [BookId] INT NOT NULL,
        [AuthorId] INT NOT NULL,
        CONSTRAINT [PK_BookAuthors] PRIMARY KEY CLUSTERED ([BookId] ASC, [AuthorId] ASC),
        CONSTRAINT [FK_BookAuthors_Books_BookId] FOREIGN KEY ([BookId]) 
            REFERENCES [dbo].[Books] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookAuthors_Authors_AuthorId] FOREIGN KEY ([AuthorId]) 
            REFERENCES [dbo].[Authors] ([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_BookAuthors_AuthorId] ON [dbo].[BookAuthors]([AuthorId] ASC);
END
GO

-- 5. جدول اعضای کتابخانه (Members)
IF OBJECT_ID(N'[dbo].[Members]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Members] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [NationalCode] VARCHAR(10) NOT NULL,
        [Phone] VARCHAR(15) NOT NULL,
        [Email] VARCHAR(150) NULL,
        [Address] NVARCHAR(500) NULL,
        [RegisterDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Members] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Members_NationalCode] ON [dbo].[Members]([NationalCode] ASC);
    CREATE NONCLUSTERED INDEX [IX_Members_LastName_FirstName] ON [dbo].[Members]([LastName] ASC, [FirstName] ASC);
    CREATE NONCLUSTERED INDEX [IX_Members_Phone] ON [dbo].[Members]([Phone] ASC);
END
GO

-- 6. جدول امانات کتاب (Loans)
IF OBJECT_ID(N'[dbo].[Loans]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Loans] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [BookId] INT NOT NULL,
        [MemberId] INT NOT NULL,
        [LoanDate] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [DueDate] DATETIME2 NOT NULL,
        [ReturnDate] DATETIME2 NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0: Active, 1: Returned, 2: Overdue, 3: Lost
        [Notes] NVARCHAR(1000) NULL,
        CONSTRAINT [PK_Loans] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Loans_Books_BookId] FOREIGN KEY ([BookId]) 
            REFERENCES [dbo].[Books] ([Id]) ON DELETE RESTRICT,
        CONSTRAINT [FK_Loans_Members_MemberId] FOREIGN KEY ([MemberId]) 
            REFERENCES [dbo].[Members] ([Id]) ON DELETE RESTRICT
    );
    CREATE NONCLUSTERED INDEX [IX_Loans_BookId] ON [dbo].[Loans]([BookId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Loans_MemberId] ON [dbo].[Loans]([MemberId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Loans_Status_DueDate] ON [dbo].[Loans]([Status] ASC, [DueDate] ASC);
END
GO

-- 7. جداول احراز هویت ASP.NET Core Identity
IF OBJECT_ID(N'[dbo].[AspNetRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetRoles] (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NULL,
        [NormalizedName] NVARCHAR(256) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]([NormalizedName] ASC) WHERE [NormalizedName] IS NOT NULL;
END
GO

IF OBJECT_ID(N'[dbo].[AspNetUsers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUsers] (
        [Id] NVARCHAR(450) NOT NULL,
        [FullName] NVARCHAR(150) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [UserName] NVARCHAR(256) NULL,
        [NormalizedUserName] NVARCHAR(256) NULL,
        [Email] NVARCHAR(256) NULL,
        [NormalizedEmail] NVARCHAR(256) NULL,
        [EmailConfirmed] BIT NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NULL,
        [SecurityStamp] NVARCHAR(MAX) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        [PhoneNumber] NVARCHAR(MAX) NULL,
        [PhoneNumberConfirmed] BIT NOT NULL,
        [TwoFactorEnabled] BIT NOT NULL,
        [LockoutEnd] DATETIMEOFFSET NULL,
        [LockoutEnabled] BIT NOT NULL,
        [AccessFailedCount] INT NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE [NormalizedUserName] IS NOT NULL;
END
GO

IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles] (
        [UserId] NVARCHAR(450) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]([RoleId] ASC);
END
GO
