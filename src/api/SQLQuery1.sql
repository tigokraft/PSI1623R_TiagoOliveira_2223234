-- Create database
CREATE DATABASE FinSync;
GO

USE FinSync;
GO

-- Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) DEFAULT 'user'
);

-- Categories Table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE
);

-- Expenses Table
CREATE TABLE Expenses (
    ExpenseId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Date DATETIME NOT NULL,
    Tags NVARCHAR(255),
    Description NVARCHAR(255),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

-- Budgets Table
CREATE TABLE Budgets (
    BudgetId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    MonthlyLimit DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

-- Goals Table
CREATE TABLE Goals (
    GoalId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    TargetAmount DECIMAL(18,2) NOT NULL,
    CurrentSaved DECIMAL(18,2) DEFAULT 0,
    Deadline DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- API Keys Table
CREATE TABLE ApiKeys (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Key] NVARCHAR(128) NOT NULL,
    Owner NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Seed Categories
INSERT INTO Categories (CategoryName) VALUES 
('Alimentação'), ('Transporte'), ('Habitação'), ('Educação');

-- Seed Admin User (Plaintext password — for dev only)
INSERT INTO Users (Username, PasswordHash, Role)
VALUES ('admin', 'admin123', 'admin');

select * from users