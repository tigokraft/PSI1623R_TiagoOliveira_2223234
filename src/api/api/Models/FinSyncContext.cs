using Microsoft.EntityFrameworkCore;
using FinSync.Models;

namespace FinSync.Data
{
    public class FinSyncContext : DbContext
    {
        public FinSyncContext(DbContextOptions<FinSyncContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Income> Incomes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Alimentação" },
                new Category { CategoryId = 2, CategoryName = "Transporte" },
                new Category { CategoryId = 3, CategoryName = "Habitação" },
                new Category { CategoryId = 4, CategoryName = "Educação" }
            );
        }
    }
}