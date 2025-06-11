using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinSync.Models
{
    public class Income
    {
        [Key]
        public int IncomeId { get; set; }

        public int UserId { get; set; } // Foreign key to User
        // public User User { get; set; } // Navigation property

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Ensure correct precision for currency
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } // The actual date this specific income occurrence happened

        [Required]
        [StringLength(255)]
        public string Descr { get; set; }
        
        // This links an income occurrence back to its recurring schedule. Null if one-time income.
        public int? RecurringScheduleId { get; set; } 
        [ForeignKey("RecurringScheduleId")]
        public RecurringIncomeSchedule? RecurringSchedule { get; set; } // Navigation property
    }
}