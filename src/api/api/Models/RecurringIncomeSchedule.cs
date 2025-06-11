using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinSync.Models
{
    public class RecurringIncomeSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public int UserId { get; set; } // Foreign key to User
        // public User User { get; set; } // Navigation property if you have a User model

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Ensure correct precision for currency
        public decimal Amount { get; set; }

        [Required]
        [StringLength(255)]
        public string Descr { get; set; }

        [Required]
        public string Recurrence { get; set; } // "daily", "weekly", "monthly", "yearly" (consider adding yearly if needed)

        [Required]
        public DateTime StartDate { get; set; } // The original start date of the recurring series

        public DateTime? EndDate { get; set; } // Nullable end date

        [Required]
        public DateTime NextOccurrenceDate { get; set; } // The date the next income *should* be generated

        public bool IsActive { get; set; } = true; // To easily deactivate schedules
    }
}