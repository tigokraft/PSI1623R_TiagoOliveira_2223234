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
                .HasIndex(c => new { c.UserId, c.CategoryName })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Income>()
                .HasIndex(i => new { i.RecurringScheduleId, i.Date })
                .IsUnique()
                .HasFilter("[RecurringScheduleId] IS NOT NULL");

            modelBuilder.Entity<Income>()
                .Property(i => i.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Budget>()
                .Property(b => b.MonthlyLimit)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Goal>()
                .Property(g => g.TargetAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Goal>()
                .Property(g => g.CurrentSaved)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RecurringIncomeSchedule>()
                .Property(ris => ris.Amount)
                .HasColumnType("decimal(18, 2)");

            // The direct mapping for CategoryId as a foreign key without a navigation property
            // is implicitly handled by EF Core if CategoryId exists on Income and CategoryId is PK on Category.
            // Explicitly adding it here to ensure it's required and has OnDelete behavior,
            // even without a direct navigation property in Income model.
            modelBuilder.Entity<Income>()
                .HasOne<Category>() // Specify the principal entity type
                .WithMany() // No navigation property on Category back to Income
                .HasForeignKey(i => i.CategoryId) // Define the foreign key
                .IsRequired() // CategoryId is a required field on Income
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete behavior for Category

            modelBuilder.Entity<Income>()
                .HasOne(i => i.RecurringSchedule)
                .WithMany()
                .HasForeignKey(i => i.RecurringScheduleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
