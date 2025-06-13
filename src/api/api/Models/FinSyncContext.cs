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
        public DbSet<RecurringIncomeSchedule> RecurringIncomeSchedules { get; set; }

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
                new Category { CategoryId = 4, CategoryName = "Educação" },
                new Category { CategoryId = 5, CategoryName = "Lazer"},
                new Category { CategoryId = 6, CategoryName = "Salário" },
                new Category { CategoryId = 7, CategoryName = "Investimentos" },
                new Category { CategoryId = 8, CategoryName = "Presentes" },
                new Category { CategoryId = 9, CategoryName = "Outros" }
            );

            modelBuilder.Entity<Income>()
                .HasIndex(i => new { i.RecurringScheduleId, i.Date })
                .IsUnique()
                .HasFilter("[RecurringScheduleId] IS NOT NULL");

            modelBuilder.Entity<Income>()
                .Property(i => i.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RecurringIncomeSchedule>()
                .Property(ris => ris.Amount)
                .HasColumnType("decimal(18, 2)");

            // Changed from WithMany(c => c.Incomes) to WithMany() to resolve CS1061 error
            // This means Category will not have a navigation property back to Income by default
            modelBuilder.Entity<Income>()
                .HasOne(i => i.Category)
                .WithMany() 
                .HasForeignKey(i => i.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Income>()
                .HasOne(i => i.RecurringSchedule)
                .WithMany()
                .HasForeignKey(i => i.RecurringScheduleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
